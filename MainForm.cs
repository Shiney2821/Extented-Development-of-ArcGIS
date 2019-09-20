using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic ;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using MapControlApplication1;


namespace CSProject
{
    public sealed partial class MainForm : Form
    {
        #region class private members
        private IMapControl3 m_mapControl = null;
        private string m_mapDocumentName = string.Empty;
        #endregion

        #region class constructor
        public MainForm()
        {
            InitializeComponent();
        }
        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            //get the MapControl
            m_mapControl = (IMapControl3)axMapControl1.Object;

            //disable the Save menu (since there is no document yet)
            menuSaveDoc.Enabled = false;
        }

        #region Main Menu event handlers
        private void menuNewDoc_Click(object sender, EventArgs e)
        {
            //execute New Document command
            ICommand command = new CreateNewDocument();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuOpenDoc_Click(object sender, EventArgs e)
        {
            //execute Open Document command
            ICommand command = new ControlsOpenDocCommandClass();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuSaveDoc_Click(object sender, EventArgs e)
        {
            //execute Save Document command
            if (m_mapControl.CheckMxFile(m_mapDocumentName))
            {
                //create a new instance of a MapDocument
                IMapDocument mapDoc = new MapDocumentClass();
                mapDoc.Open(m_mapDocumentName, string.Empty);

                //Make sure that the MapDocument is not readonly
                if (mapDoc.get_IsReadOnly(m_mapDocumentName))
                {
                    MessageBox.Show("Map document is read only!");
                    mapDoc.Close();
                    return;
                }

                //Replace its contents with the current map
                mapDoc.ReplaceContents((IMxdContents)m_mapControl.Map);

                //save the MapDocument in order to persist it
                mapDoc.Save(mapDoc.UsesRelativePaths, false);

                //close the MapDocument
                mapDoc.Close();
            }
        }

        private void menuSaveAs_Click(object sender, EventArgs e)
        {
            //execute SaveAs Document command
            ICommand command = new ControlsSaveAsDocCommandClass();
            command.OnCreate(m_mapControl.Object);
            command.OnClick();
        }

        private void menuExitApp_Click(object sender, EventArgs e)
        {
            //exit the application
            Application.Exit();
        }
        #endregion

        //listen to MapReplaced evant in order to update the statusbar and the Save menu
        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            //get the current document name from the MapControl
            m_mapDocumentName = m_mapControl.DocumentFilename;

            //if there is no MapDocument, diable the Save menu and clear the statusbar
            if (m_mapDocumentName == string.Empty)
            {
                menuSaveDoc.Enabled = false;
                statusBarXY.Text = string.Empty;
            }
            else
            {
                //enable the Save manu and write the doc name to the statusbar
                menuSaveDoc.Enabled = true;
                statusBarXY.Text = System.IO.Path.GetFileName(m_mapDocumentName);
            }
        }

        /**************************************************
         polygon绘制及编辑功能相关成员变量声明与初始化
         **************************************************/
        int m_marker = 0;//标记符
        IDisplayFeedback m_DisplayFeedback = null;//polygon的绘制及编辑的显示反馈
        IGraphicsContainer m_graphicsContainer;//图形容器
        IPolygon m_polygon = new PolygonClass();//被选中的polygon
        IElement m_hitElement = new PolygonElementClass();//被选中的element
        IPolyline m_polyline = new PolylineClass();//绘制过程中的红线
        IPoint m_point = new PointClass();//绘制polygon时的第一个点
        IElement m_element_red = null;//绘制过程中的红线所对应的element
        IElement m_element_green = null;//绘制过程中的蓝线所对应的element
        IPointCollection m_PointColl = new MultipointClass();//被选择的polygon的点集
        List<IElement> m_list = new List<IElement>();//存放被选中的polygon的点的list
        /*********************************
         Snap功能相关成员变量声明与初始化
         *********************************/
        ITextElement m_textelement = null;//捕捉点所在图层显示气泡z
        IElement m_element_snap = null;//当前鼠标点
        IPoint m_currentPoint = new PointClass();//捕捉到得点
        IPoint m_snapPoint = null;//移动点反馈对象
        IMovePointFeedback m_movePointFeedback = new MovePointFeedbackClass();//捕捉图层
        int m_flag = 0;//有无捕捉到点


        //鼠标移动触发函数
        private void axMapControl1_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            statusBarXY.Text = string.Format("{0}, {1}  {2}", e.mapX.ToString("#######.##"), e.mapY.ToString("#######.##"), axMapControl1.MapUnits.ToString().Substring(4));

            //橡皮条实现
            IPoint p_point = new PointClass();
            p_point.PutCoords(e.mapX, e.mapY);
            if (this.m_DisplayFeedback != null)
            {
                this.m_DisplayFeedback.MoveTo(p_point);
            }
            //Snap功能实现
            if (miStartSnap.Checked)
            {
                //检测GraphicsContainer中是否已加入m_element_snap和m_textelement
                if (m_flag != 0)
                {
                    m_flag = 0;
                    axMapControl1.ActiveView.GraphicsContainer.DeleteElement(m_element_snap);
                    axMapControl1.ActiveView.GraphicsContainer.DeleteElement((IElement)m_textelement);
                }
                //初始化记录用p_layer/m_snapPoint/m_currentPoint
                ILayer p_Layer = axMapControl1.get_Layer(0);
                m_snapPoint = null;
                m_currentPoint.PutCoords(e.mapX, e.mapY);
                //历遍所有图层捕捉boundary
                for (int i = 0; i < axMapControl1.LayerCount; i++)
                {
                    //记录当前被测试的layer并转化为IFeatureLayer
                    ILayer p_testLayer = axMapControl1.get_Layer(i);
                    IFeatureLayer p_featureLayer = p_testLayer as IFeatureLayer;
                    //鼠标光标自动捕获顶点
                    m_snapPoint = Snapping(e.mapX, e.mapY, p_featureLayer);
                    //判断是否捕捉到点
                    if (m_snapPoint != null)
                    {
                        //如果捕捉到点就记录该图层并跳出循环
                        p_Layer = p_testLayer;
                        break;
                    }
                }
                //判断是否捕捉到点
                if (m_snapPoint != null)
                {
                    //生成图层提示气泡并加入GraghicsContainer
                    m_textelement = createTextElement(e.mapX, e.mapY, p_Layer.Name);
                    axMapControl1.ActiveView.GraphicsContainer.AddElement(m_textelement as IElement, 1);
                    //建立element高亮选择点并加入GraphicsContainer
                    ISymbol p_symbol = CreateMarkerElement(m_currentPoint, p_Layer);
                    axMapControl1.ActiveView.GraphicsContainer.AddElement(m_element_snap, 0);
                    //标记为已加入
                    m_flag++;
                    //设置m_movePointFeedback
                    IGeometry p_geometry = m_element_snap.Geometry;
                    m_movePointFeedback.Display = axMapControl1.ActiveView.ScreenDisplay;
                    m_movePointFeedback.Symbol = p_symbol;
                    m_movePointFeedback.Start(p_geometry as IPoint, m_currentPoint);
                    //移动element至捕捉到的点
                    ElementMoveTo(m_snapPoint);
                }
            }
        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1)
            {
                //给m_graphicsContainer赋值
                m_graphicsContainer = axMapControl1.Map as IGraphicsContainer;
                //在添加polygon被选中时
                if (miAddPolygon.Checked)
                {
                    //在鼠标点击的位置生成一个点   
                    IPoint p_Point = new PointClass();
                    p_Point.PutCoords(e.mapX, e.mapY);
                    IActiveView p_activeview = axMapControl1.Map as IActiveView;
                    IScreenDisplay p_ScreenDisplay = p_activeview.ScreenDisplay;
                    //标记符等于0时
                    if (m_marker == 0)
                    {
                        //对m_DisplayFeedback重新初始化
                        m_DisplayFeedback = new NewPolygonFeedbackClass();
                        //设置m_DisplayFeedback的display
                        m_DisplayFeedback.Display = p_ScreenDisplay;
                        //开始绘制polygon的第一个点
                        ((INewPolygonFeedback)m_DisplayFeedback).Start(p_Point);
                        //将m_polyline转为pointcollection
                        IPointCollection p_PointCollection = m_polyline as IPointCollection;
                        //将第一个点加入pointcollection中
                        p_PointCollection.AddPoint(p_Point);
                        //将第一个点记录到m_point中
                        m_point = p_Point;
                        //对m_polyline重新赋值
                        m_polyline = p_PointCollection as IPolyline;
                        if (m_marker == 0)
                        {
                            m_marker = 1;
                        }
                        //刷新
                        axMapControl1.ActiveView.Refresh();
                    }
                    //如果不是第一次点击,就添加节点  
                    else
                    {
                        //初始化红线和蓝线
                        if (m_element_red != null)
                        {
                            m_graphicsContainer.DeleteElement(m_element_red);
                        }
                        if (m_element_green != null)
                        {
                            m_graphicsContainer.DeleteElement(m_element_green);
                        }
                        //向m_DisplayFeedback中添加顶点
                        ((INewPolygonFeedback)m_DisplayFeedback).AddPoint(p_Point);
                        IPointCollection p_PointCollection = m_polyline as IPointCollection;
                        p_PointCollection.AddPoint(p_Point);
                        //生成红线
                        m_polyline = p_PointCollection as IPolyline;
                        ILineElement p_LineElement_red = new LineElementClass();
                        m_element_red = p_LineElement_red as IElement;
                        m_element_red.Geometry = m_polyline;
                        ISimpleLineSymbol p_SimpleLineSymbol = new SimpleLineSymbolClass();
                        IRgbColor p_rgbcolor = new RgbColorClass();
                        p_rgbcolor.Red = 255;
                        p_rgbcolor.Green = 0;
                        p_rgbcolor.Blue = 0;
                        p_SimpleLineSymbol.Color = p_rgbcolor as IColor;
                        p_LineElement_red.Symbol = p_SimpleLineSymbol as ILineSymbol;
                        //设置蓝线symbol
                        ISimpleLineSymbol p_SimpleLine_green = new SimpleLineSymbolClass();
                        IRgbColor p_rgbcolor_green = new RgbColorClass();
                        p_rgbcolor_green.Red = 0;
                        p_rgbcolor_green.Green = 255;
                        p_rgbcolor_green.Blue = 255;
                        p_SimpleLine_green.Color = p_rgbcolor_green as IColor;
                        //生成蓝线                        
                        IPolyline p_polyline = new PolylineClass();
                        IPointCollection p_pointColl_green = p_polyline as IPointCollection;
                        //向蓝线中添加两个顶点
                        p_pointColl_green.AddPoint(m_point);
                        p_pointColl_green.AddPoint(p_Point);
                        p_polyline = p_pointColl_green as IPolyline;
                        ILineElement p_LineElement_green = new LineElementClass();
                        m_element_green = p_LineElement_green as IElement;
                        p_LineElement_green.Symbol = p_SimpleLine_green as ILineSymbol;
                        m_element_green.Geometry = p_polyline;
                        //将红线和蓝线加入containner显示
                        m_graphicsContainer.AddElement(m_element_red, 0);
                        m_graphicsContainer.AddElement(m_element_green, 0);
                        //更新视图
                        axMapControl1.ActiveView.Refresh();
                    }
                }
                //当选择polygon被选中时
                if (miSelectPolygon.Checked)
                {
                    //如果m_list不为空，则从容器中把其所对应element删除并将m_list清空
                    if (m_list.Count != 0)
                    {
                        for (int i = 0; i < m_list.Count; i++)
                        {
                            m_graphicsContainer.DeleteElement(m_list[i]);
                        }
                        m_list.Clear();
                    }
                    //刷新
                    axMapControl1.ActiveView.Refresh();
                    IPoint p_point = new PointClass();
                    //记录点下的点
                    p_point.PutCoords(e.mapX, e.mapY);
                    //得到选中的element集合
                    IEnumElement p_EnumElement = m_graphicsContainer.LocateElements(p_point, 5);
                    IGeometry p_geometry;
                    //选中的element集合不为空时
                    if (p_EnumElement != null)
                    {
                        m_hitElement = p_EnumElement.Next();
                        p_geometry = m_hitElement.Geometry;
                        //循环找到类型为polygon的element
                        while (p_geometry.GeometryType != esriGeometryType.esriGeometryPolygon && m_hitElement != null)
                        {
                            m_hitElement = p_EnumElement.Next();
                            if (m_hitElement != null)
                            {
                                p_geometry = m_hitElement.Geometry;
                            }
                        }
                        if (p_geometry.GeometryType == esriGeometryType.esriGeometryPolygon)
                        {
                            m_polygon = (IPolygon)p_geometry;
                            m_PointColl = m_polygon as IPointCollection;
                            //循环将选中的polygon的每个点以element加入容器中
                            for (int k = 1; k < m_PointColl.PointCount; k++)
                            {
                                IMarkerElement p_MarkerElement = new MarkerElementClass();
                                ISimpleMarkerSymbol p_simpleMarker = new SimpleMarkerSymbolClass();
                                p_simpleMarker.Size = 5;

                                IElement p_element = (IElement)p_MarkerElement;
                                p_MarkerElement.Symbol = p_simpleMarker as IMarkerSymbol;

                                if (!(p_element == null))
                                {
                                    p_element.Geometry = m_PointColl.get_Point(k);

                                    m_graphicsContainer.AddElement(p_element, 0);

                                }
                                m_list.Add(p_element);
                            }
                            axMapControl1.ActiveView.Refresh();
                        }
                    }
                }
                //当编辑polygon被选中时
                if (miEditPolygon.Checked)
                {
                    if (m_polygon != null)
                    {
                        //在鼠标点击的位置生成一个点 
                        IPoint p_Point = new PointClass();
                        p_Point.PutCoords(e.mapX, e.mapY);
                        IActiveView p_ActiveView = axMapControl1.Map as IActiveView;
                        IScreenDisplay p_ScreenDisplay = p_ActiveView.ScreenDisplay;
                        //建立新的hittest，设置相关变量，获取点击处polygon的顶点
                        IHitTest p_HitTest = m_polygon as IHitTest;
                        IPoint p_hitPoint = new PointClass();
                        double p_distance = 0;
                        bool p_isOnRightSide = true;
                        int p_hitPartIndex = 0;
                        int p_hitSegmentIndex = 0;
                        bool p_isHit = p_HitTest.HitTest(p_Point, this.axMapControl1.ActiveView.Extent.Width / 100, esriGeometryHitPartType.esriGeometryPartVertex, p_hitPoint, ref p_distance, ref p_hitPartIndex, ref p_hitSegmentIndex, ref p_isOnRightSide);
                        //如果有点被选中，开始拖动该点
                        if (p_isHit)
                        {
                            m_DisplayFeedback = new PolygonMovePointFeedbackClass();
                            m_marker = 2;
                            m_DisplayFeedback.Display = p_ScreenDisplay;
                            ((IPolygonMovePointFeedback)m_DisplayFeedback).Start(m_polygon, p_hitSegmentIndex, p_Point);
                        }
                    }
                }
            }
        }

        private void axMapControl1_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            //移动多边形节点
            //当“编辑polygon”被勾选且标记符为2时
            if (miEditPolygon.Checked && m_marker == 2)
            {
                //删除原多边形的顶点
                if (m_list.Count != 0)
                {
                    for (int j = 0; j < m_list.Count; j++)
                    {
                        m_graphicsContainer.DeleteElement(m_list[j]);
                    }
                    m_list.Clear();
                }
                //停止绘制已选中的polygon
                m_polygon = ((IPolygonMovePointFeedback)m_DisplayFeedback).Stop();
                //如果新polygon不为空，更新被存放的多边形
                if (m_polygon != null)
                {
                    m_hitElement.Geometry = m_polygon;
                    m_graphicsContainer.UpdateElement(m_hitElement);
                }
                //将新多边形的点放入新的点集存放并显示
                m_PointColl = m_polygon as IPointCollection;
                for (int k = 1; k < m_PointColl.PointCount; k++)
                {
                    IMarkerElement p_markerelement = new MarkerElementClass();
                    ISimpleMarkerSymbol p_simplemarkersymbol = new SimpleMarkerSymbolClass();
                    p_simplemarkersymbol.Size = 5;

                    IElement p_element = (IElement)p_markerelement;
                    p_markerelement.Symbol = p_simplemarkersymbol as IMarkerSymbol;

                    if (p_element != null)
                    {
                        p_element.Geometry = m_PointColl.get_Point(k);

                        m_graphicsContainer.AddElement(p_element, 0);

                    }
                    m_list.Add(p_element);
                }
                this.axMapControl1.Refresh(esriViewDrawPhase.esriViewGraphics, null, null);
                //重置编辑符为零
                m_marker = 0;
            }
        }

        private void axMapControl1_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {
            //当“添加polygon”被勾选时，结束polygon的绘制
            if (miAddPolygon.Checked)
            {
                //结束反馈
                IPolygon p_polygon = ((INewPolygonFeedback)m_DisplayFeedback).Stop();
                //当多边形不为空
                if (p_polygon != null)
                {
                    //IGraphicsContainer转换接口，用于在当前地图存放多边形
                    m_graphicsContainer = (IGraphicsContainer)(axMapControl1.Map);
                    IActiveView p_activeView = axMapControl1.Map as IActiveView;
                    IScreenDisplay screenDisplay = p_activeView.ScreenDisplay;
                    //设置多边形的符号
                    ISimpleFillSymbol p_simpleFillSymbol = new SimpleFillSymbolClass();
                    IRgbColor p_rgbColor = new RgbColorClass();
                    p_rgbColor.Red = 255;
                    p_rgbColor.Green = 255;
                    IColor p_color = p_rgbColor;
                    p_color.Transparency = 0;
                    p_simpleFillSymbol.Color = p_color;
                    //新建element对象存放多边形
                    IPolygonElement p_polygonElement = new PolygonElementClass();
                    IElement p_element = (IElement)p_polygonElement;
                    //将多边形以element的形式储存在graphisContainer中
                    if (p_element != null)
                    {
                        p_element.Geometry = p_polygon;
                        IFillShapeElement pFillShapeElement = p_element as IFillShapeElement;
                        pFillShapeElement.Symbol = p_simpleFillSymbol;
                        m_graphicsContainer.AddElement(p_element, 0);
                    }
                    //更改标记符为零
                    m_marker = 0;
                    //取消勾选“添加polygon”
                    miAddPolygon.Checked = false;
                    if (m_element_red != null)
                    {
                        m_graphicsContainer.DeleteElement(m_element_red);
                    }
                    if (m_element_green != null)
                    {
                        m_graphicsContainer.DeleteElement(m_element_green);
                    }
                    m_polyline = new PolylineClass();
                    m_point = new PointClass();
                    m_element_red = null;
                    m_element_green = null;
                    p_activeView.Refresh();
                }
                //如果多边形为空，初始化所有变量
                else
                {
                    if (m_element_red != null)
                    {
                        m_graphicsContainer.DeleteElement(m_element_red);
                    }
                    if (m_element_green != null)
                    {
                        m_graphicsContainer.DeleteElement(m_element_green);
                    }
                    m_hitElement = null;
                    m_DisplayFeedback = null;
                    m_marker = 0;
                    m_polyline = new PolylineClass();
                    m_point = new PointClass();
                    m_element_red = null;
                    m_element_green = null;
                    axMapControl1.ActiveView.Refresh();
                }
            }
        }

        //设置点击“添加polygon”菜单项的变化
        private void miPolygonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //未被勾选时勾选，并保证“编辑polygon”、“选择polygon”未被勾选
            if (!miAddPolygon.Checked)
            {
                miAddPolygon.Checked = true;
                miSelectPolygon.Checked = false;
                miEditPolygon.Checked = false;
                //当已有选择的多边形，取消显示其点集
                if (m_list.Count != 0)
                {
                    for (int i = 0; i < m_list.Count; i++)
                    {
                        m_graphicsContainer.DeleteElement(m_list[i]);
                    }
                    m_list.Clear();
                }
                //初始化变量
                m_hitElement = null;
                m_DisplayFeedback = null;
                axMapControl1.ActiveView.Refresh();
            }
            //当勾选时取消勾选勾选
            else
            {
                miAddPolygon.Checked = false;
            }
        }

        //设置点击“选择polygon”菜单项的变化
        private void miSelectPolygon_Click(object sender, EventArgs e)
        {
            //当未被勾选时勾选，并保证“添加polygon”未被勾选
            if (!miSelectPolygon.Checked)
            {
                miSelectPolygon.Checked = true;
                if (miAddPolygon.Checked)
                    miAddPolygon.Checked = false;
            }
            else   //当勾选时取消勾选勾选，并使之前选择的多边形的顶点消失
            {
                miSelectPolygon.Checked = false;
                if (m_list.Count != 0)
                {
                    for (int i = 0; i < m_list.Count; i++)
                    {
                        m_graphicsContainer.DeleteElement(m_list[i]);
                    }
                    m_list.Clear();
                }
                axMapControl1.ActiveView.Refresh();
            }
        }
        //设置点击“编辑polygon”菜单项的变化
        private void miEditPolygon_Click(object sender, EventArgs e)
        {
            //未被勾选时勾选，并保证“添加polygon”未被勾选当
            if (!miEditPolygon.Checked)
            {
                miEditPolygon.Checked = true;
                if (miAddPolygon.Checked)
                    miAddPolygon.Checked = false;
            }
            //当勾选时取消勾选勾选，初始化hitElement和DisplayFeedback 
            else
            {
                miEditPolygon.Checked = false;
                m_hitElement = null;
                m_DisplayFeedback = null;
            }
        }
        //捕捉当前位置周围boundary
        public IPoint Snapping(double x, double y, IFeatureLayer featureLayer)
        {
            //记录当前图层的featureClass
            IFeatureClass p_featureClass = featureLayer.FeatureClass;
            //记录hitpoint结果
            IPoint p_hitPoint = new PointClass();
            //记录hitpoint所用GeometryCollection
            IGeometryCollection p_GeometryCol = new GeometryBagClass();
            //临时记录单个feature
            IFeature p_feature;
            //判断featureclass类型
            int p_flag = 0;
            switch (p_featureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    p_GeometryCol = new MultipointClass() as IGeometryCollection;
                    p_flag = 0;
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    p_GeometryCol = new PolygonClass() as IGeometryCollection;
                    p_flag = 1;
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    p_GeometryCol = new PolylineClass() as IGeometryCollection;
                    p_flag = 1;
                    break;
            }
            IFeatureCursor p_featurecursor = p_featureClass.Search(null, false);
            //按照featureclass的类型用不同方法加入feature
            if (p_flag == 0)
            {
                object missing = Type.Missing;
                //使用featurecursor选择feature提速
                p_feature = p_featurecursor.NextFeature();
                while (p_feature != null)
                {

                    p_GeometryCol.AddGeometry(p_feature.Shape, ref missing, ref missing);
                    p_feature = p_featurecursor.NextFeature();
                }
            }
            if (p_flag == 1)
            {
                p_feature = p_featurecursor.NextFeature();
                while (p_feature != null)
                {
                    p_GeometryCol.AddGeometryCollection((IGeometryCollection)p_feature.Shape);
                    p_feature = p_featurecursor.NextFeature();
                }
            }

            //创建hittest任务
            IHitTest p_hitTest = p_GeometryCol as IHitTest;
            //创建并初始化传出参数
            double p_hitDist = 0;
            int p_partIndex = 0;
            int p_vertexIndex = 0;
            bool p_bVertexHit = false;
            //设置测试范围
            double p_tol = convertPixelsToMapUnits(axMapControl1.ActiveView, 3);
            if (p_hitTest.HitTest(m_currentPoint, p_tol, esriGeometryHitPartType.esriGeometryPartBoundary, p_hitPoint, ref p_hitDist, ref p_partIndex, ref p_vertexIndex, ref p_bVertexHit))
            {
                //若捕捉到点则返回点
                return p_hitPoint;
            }
            //否则返回null
            return null;
        }
        //创建element
        public ISymbol CreateMarkerElement(IPoint point, ILayer pLayer)
        {
            //获取当前图层feature类型
            IFeatureLayer p_featureLayer = pLayer as IFeatureLayer;
            IFeatureClass p_featureClass = p_featureLayer.FeatureClass;
            //建立一个marker元素
            IMarkerElement p_markerElement = new MarkerElement() as IMarkerElement;
            ISimpleMarkerSymbol p_simpleMarkerSymbol = new SimpleMarkerSymbol();
            //符号化元素
            p_simpleMarkerSymbol.Color = getRGB(255, 0, 0);
            p_simpleMarkerSymbol.Outline = true;
            p_simpleMarkerSymbol.OutlineColor = getRGB(0, 255, 0) as IColor;
            p_simpleMarkerSymbol.OutlineSize = 5;
            //分类设置
            if (p_featureClass.ShapeType == esriGeometryType.esriGeometryPoint)
            {
                p_simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
            }
            else if (p_featureClass.ShapeType == esriGeometryType.esriGeometryPolygon)
            {
                p_simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;
            }
            else if (p_featureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
            {
                p_simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSSquare;
            }
            else
            {
                p_simpleMarkerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
            }
            //设置m_element_snap的属性
            p_markerElement.Symbol = p_simpleMarkerSymbol;
            m_element_snap = p_markerElement as IElement;
            m_element_snap.Geometry = point as IGeometry;
            //设置返回值
            ISymbol symbol = p_simpleMarkerSymbol as ISymbol;
            symbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
            return symbol;
        }
        //通过设置RGB创建一种颜色
        private IRgbColor getRGB(int r, int g, int b)
        {
            IRgbColor p_Color = new RgbColorClass();
            p_Color.Red = r;  //设置红色
            p_Color.Green = g;   //设置绿色
            p_Color.Blue = b;   //设置蓝色
            return p_Color;
        }
        //移动元素到新的位置
        public void ElementMoveTo(IPoint point)
        {
            //移动元素
            m_movePointFeedback.MoveTo(point);
            IGeometry p_geometry = null;
            if (m_element_snap != null)
            {
                p_geometry = m_movePointFeedback.Stop();
                m_element_snap.Geometry = p_geometry;
                //更新该元素的位置
                axMapControl1.ActiveView.GraphicsContainer.UpdateElement(m_element_snap);
                //重新移动元素
                m_movePointFeedback.Stop();
                //刷新视图
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
        }
        //通过名称获取图层
        public ILayer GetLayerByName(string layerName, AxMapControl axMapControl1)
        {
            //历遍所有图层查找图层名相同的图层
            for (int i = 0; i < axMapControl1.LayerCount; i++)
            {
                if (axMapControl1.get_Layer(i).Name.Equals(layerName))
                {
                    return axMapControl1.get_Layer(i);
                }
            }
            //若没有则返回null
            return null;
        }

        //转换像素到地图单位
        public double convertPixelsToMapUnits(IActiveView activeView, double pixelUnits)
        {
            double p_realDisplayExtent;
            int p_pixelExtent;
            double sizeOfOnePixel;
            //记录窗口和地图分别的宽度
            p_pixelExtent = activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().right - activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().left;
            p_realDisplayExtent = activeView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            //返回每个pixel对应的地图块大小
            sizeOfOnePixel = p_realDisplayExtent / p_pixelExtent;
            return pixelUnits * sizeOfOnePixel;
        }
        //snap功能开关
        private void miStartSnap_Click(object sender, EventArgs e)
        {
            //打开开关并初始化成员变量
            if (!miStartSnap.Checked)
            {
                miStartSnap.Checked = true;
                m_element_snap = null;//当前鼠标点
                m_currentPoint = new PointClass();//捕捉到得点
                m_snapPoint = null;//移动点反馈对象
                m_movePointFeedback = new MovePointFeedbackClass();//捕捉图层
                m_flag = 0;
            }
            else//关闭开关并删除GraphicsContainer中的element
            {
                miStartSnap.Checked = false;
                if (m_flag != 0)
                {
                    axMapControl1.ActiveView.GraphicsContainer.DeleteElement(m_element_snap);
                    axMapControl1.ActiveView.GraphicsContainer.DeleteElement((IElement)m_textelement);
                    axMapControl1.ActiveView.Refresh();
                }
            }
        }
        //设置文字气泡位置和背景属性
        public IBalloonCallout createBalloonCallout(double x, double y)
        {
            IRgbColor p_rgbcolor = new RgbColorClass();  //设置浅黄色
            {
                p_rgbcolor.Red = 255;
                p_rgbcolor.Green = 255;
                p_rgbcolor.Blue = 200;
            }
            ISimpleFillSymbol p_simplefillsymbol = new SimpleFillSymbolClass(); //设置填充符号
            {
                p_simplefillsymbol.Color = p_rgbcolor;
                p_simplefillsymbol.Style = esriSimpleFillStyle.esriSFSSolid;
            }
            IPoint p_point = new PointClass();   //获取鼠标的位置
            {
                p_point.PutCoords(x, y);
            }
            IBalloonCallout p_ballooncallout = new BalloonCalloutClass();  //新建并设置文字气泡对象
            {
                p_ballooncallout.Style = esriBalloonCalloutStyle.esriBCSRoundedRectangle;
                p_ballooncallout.Symbol = p_simplefillsymbol;
                p_ballooncallout.LeaderTolerance = 10;
                p_ballooncallout.AnchorPoint = p_point;
            }
            return p_ballooncallout;
        }
        //创建一个文字气泡
        public ITextElement createTextElement(double x, double y, string text)
        {
            //设置文字气泡背景位置与属性
            IBalloonCallout p_ballooncallout = createBalloonCallout(x, y);
            //新建并设置文字气泡边框、大小
            IRgbColor p_rgbcolor = new RgbColorClass();
            {
                p_rgbcolor.Green = 255;
            }
            ITextSymbol p_textsymbol = new TextSymbolClass();
            {
                p_textsymbol.Color = p_rgbcolor;
            }
            IFormattedTextSymbol p_formattedtextsymbol = p_textsymbol as IFormattedTextSymbol;
            {
                p_formattedtextsymbol.Background = p_ballooncallout as ITextBackground;
            }
            p_textsymbol.Size = 8;
            //设置气泡位置
            IPoint p_point = new PointClass();
            {
                double p_width = axMapControl1.Extent.Width / 13;
                double p_height = axMapControl1.Extent.Height / 20;
                p_point.PutCoords(x + p_width, y + p_height);
            }
            //创建element设置text属性
            ITextElement p_textelement = new TextElementClass();
            p_textelement.Symbol = p_textsymbol;
            p_textelement.Text = text;
            IElement p_element = p_textelement as IElement;
            p_element.Geometry = p_point;
            return p_textelement;
        }

        private void miPoint_Click(object sender, EventArgs e)
        {
            IPoint point = new PointClass();
            point.PutCoords(200, 100);
            IMarkerElement pMarkerElement;
            pMarkerElement = new MarkerElementClass();
            IElement pElement;
            pElement = pMarkerElement as IElement;
            pElement.Geometry = point;
            IGraphicsContainer pGraphicsContainer = axMapControl1.Map as IGraphicsContainer;
            IColor color = new RgbColorClass();
            color.RGB = 100;
            pGraphicsContainer.AddElement((IElement)pMarkerElement, 0);
            axMapControl1.ActiveView.Refresh();
        }

        private void miGeometry_Click(object sender, EventArgs e)
        {
            DataOperator dataoperator = new DataOperator(axMapControl1.Map);
            //ILayer layer;
            //layer = dataoperator.GetLayerByName("DLTB");
            //IFeatureLayer featureLayer = layer as IFeatureLayer;
            //IFeatureCursor featureCursor = featureLayer.Search(null, false);
            //IFeature feature = featureCursor.NextFeature();
            IGeometry geometry = new PolygonClass();
            geometry = m_polygon;

            IPointCollection pointCollection = (Polygon)geometry;
            DataTable dataTable = new DataTable();
            DataColumn dataColumn = new DataColumn();//创建列

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "number";//设置第一列，表示用户指定的列名
            dataColumn.DataType = System.Type.GetType("System.String");
            dataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "X";//设置第二列为在目标图层类作为分类标准的属性
            dataColumn.DataType = System.Type.GetType("System.String");
            dataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Y";//设置第三列为在目标图层类作为分类标准的属性
            dataColumn.DataType = System.Type.GetType("System.String");
            dataTable.Columns.Add(dataColumn);


            IGraphicsContainer pGraphicsContainer = axMapControl1.Map as IGraphicsContainer;
            DataRow dataRow;
            for (int i = 0; i < pointCollection.PointCount; i++)
            {

                dataRow = dataTable.NewRow();
                dataRow[0] = i;
                dataRow[1] = pointCollection.Point[i].X;
                dataRow[2] = pointCollection.Point[i].Y;//将统计结果添加到第三列
                dataTable.Rows.Add(dataRow);
                IMarkerElement pMarkerElement = new MarkerElementClass();
                IElement pElement = pMarkerElement as IElement;
                pElement.Geometry = pointCollection.Point[i];
                pGraphicsContainer.AddElement((IElement)pMarkerElement, 0);
            }
            axMapControl1.ActiveView.Refresh();

            DataBoard dataBoard = new DataBoard("Position", dataTable);
            dataBoard.Show();
        }

        private void miPolygon_Click(object sender, EventArgs e)
        {
            DataOperator dataoperator = new DataOperator(axMapControl1.Map);
            //ILayer layer;
            //layer = dataoperator.GetLayerByName("DLTB");
            //IFeatureLayer featureLayer = layer as IFeatureLayer;
            //IFeatureCursor featureCursor = featureLayer.Search(null, false);
            //IFeature feature = featureCursor.NextFeature();
            IGeometry geometry = new PolygonClass();
            geometry = m_polygon;

            IPointCollection pointCollection = (Polygon)geometry;

            /*IPoint point1 = new PointClass();
            point1.PutCoords(100, 100);
            IPoint point2 = new PointClass();
            point2.PutCoords(150, 150);
            IPoint point3 = new PointClass();
            point3.PutCoords(200, 100);
            IPointCollection pointCollection=null;
            object miss = Type.Missing;
            
            pointCollection.AddPoint(point1, ref miss, ref miss);
            pointCollection.AddPoint(point2, ref miss, ref miss);
            pointCollection.AddPoint(point3, ref miss, ref miss);*/
            IGeometry polygon = pointCollection as IGeometry;
            IPolygonElement polygonElement = new PolygonElementClass();
            IElement pElement;
            pElement = polygonElement as IElement;
            pElement.Geometry = polygon;
            IGraphicsContainer pGraphicsContainer = axMapControl1.Map as IGraphicsContainer;
            pGraphicsContainer.AddElement((IElement)polygonElement, 0);
            axMapControl1.ActiveView.Refresh();
        }
        public void setButton_miShowTime(bool choice)
        {
            miShowTime.Enabled = choice;

        }

        private void miShowTime_Click(object sender, EventArgs e)
        {
            //====将shp中的要素转换成Geometry对象========
            GeometryOperator geometryOperator = new GeometryOperator(axMapControl1.Map, this);
            DataOperator dataoperator = new DataOperator(axMapControl1.Map);
            //ILayer layer;
            //layer = dataoperator.GetLayerByName("DLTB");
            //IFeatureLayer featureLayer = layer as IFeatureLayer;
            //IFeatureCursor featureCursor = featureLayer.Search(null, false);
            //IFeature feature = featureCursor.NextFeature();
            IGeometry geometry = new PolygonClass();

            geometry = m_polygon;

            //====开始调用GeometryOperator类库中的函数
            geometryOperator.ShowGeometry(geometry);
            axMapControl1.ActiveView.Refresh();
        }

        private void axMapControl1_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            //打开开关并初始化成员变量
            if (e.keyCode == 67 || e.keyCode == 99)
            {
                if (!miStartSnap.Checked)
                {
                    miStartSnap.Checked = true;
                    m_element_snap = null;//当前鼠标点
                    m_currentPoint = new PointClass();//捕捉到得点
                    m_snapPoint = null;//移动点反馈对象
                    m_movePointFeedback = new MovePointFeedbackClass();//捕捉图层
                    m_flag = 0;
                }
                else//关闭开关并删除GraphicsContainer中的element
                {
                    miStartSnap.Checked = false;
                    if (m_flag != 0)
                    {
                        axMapControl1.ActiveView.GraphicsContainer.DeleteElement(m_element_snap);
                        axMapControl1.ActiveView.GraphicsContainer.DeleteElement((IElement)m_textelement);
                        axMapControl1.ActiveView.Refresh();
                    }
                }

            }
        }

        private void AreaStatistics_Click(object sender, EventArgs e)
        {
            StatisticAnalyze statisticAnalyze = new StatisticAnalyze(axMapControl1.Map);
            IFeatureClass featureClass = statisticAnalyze.CreateShapefile("D:\\", "deodatabase", "Clip_Polygon");
            if (featureClass == null)
            {
                MessageBox.Show("失败");
                return;
            }
            bool bRes = statisticAnalyze.AddFeatureClassToMap(featureClass, "Clip_Polygon");
            if (!bRes)
            {
 
                MessageBox.Show("加入");
                return;
            }

            statisticAnalyze.AddPolygonToLayer(m_polygon);

            statisticAnalyze.cilpOperator(axMapControl1.Map, "XZDW", "Clip_Polygon", "D:\\");
            statisticAnalyze.cilpOperator(axMapControl1.Map, "DLTB", "Clip_Polygon", "D:\\");       
            statisticAnalyze.cilpOperator(axMapControl1.Map, "LXDW", "Clip_Polygon", "D:\\");

            statisticAnalyze.DLTBquitXZandLX("DLTB_Clip", "XZDW_Clip", "LXDW_Clip");

            DataBoard databoard = new DataBoard("面积统计",statisticAnalyze.GetSt());
            databoard.Show();


        }

        private void miDeleteLayer_Click(object sender, EventArgs e)
        {
            StatisticAnalyze statisticAnalyze = new StatisticAnalyze(axMapControl1.Map);
            statisticAnalyze.DeleteFeatureDataset("D:\\", "deodatabase", "Clip_Polygon");
            
        }

        private void axTOCControl1_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {

        }
    }
}