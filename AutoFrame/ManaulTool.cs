using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFrameDll;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace AutoFrame
{
    public static class ManaulTool
    {
        public class ControlSort : IComparer<Control>
        {
            //IComparer<T> 接口:定义类型为比较两个对象而实现的方法。  
            public int Compare(Control x, Control y) //为界面的按钮排序
            {
                if (x.Location.X > y.Location.X)
                {
                    return 1;
                }
                else if(x.Location.X == y.Location.X)
                {
                    return x.Location.Y > y.Location.Y ? 1:-1;
                }
                else
                {
                    return -1;
                }
            }
        }
        
        /// <summary>
        /// 通过窗体对象得到相应工站对象
        /// </summary>
        /// <param name="frm">窗体对象</param>
        /// <returns></returns>
        public static StationBase GetStation(Form frm)
        {
            StationBase s =  StationMgr.GetInstance().GetStation(frm);
            if (s == null)
            {
                string str = string.Format("手动调试时{0}找不到对应的站位类可用，配置不匹配",frm.Text);
                MessageBox.Show(str);
                return null;
            }
            return s;
        }

        /// <summary>
        /// 更新内存中的点位参数到站位表格
        /// </summary>
        /// <param name="s">站位对象</param>
        /// <param name="grid">表格控件</param>
        public static void UpdateGrid(StationBase s, DataGridView grid)
        {

            if (s.m_dicPoint.Count > 0)
            {
                foreach (KeyValuePair<int, PointInfo> kvp in s.m_dicPoint)
                {
                    int k = 0;
                    grid.Rows.Add();
                    int j = grid.Rows.Count - 2;

                    grid.Rows[j].Cells[k++].Value = kvp.Key.ToString();
                    grid.Rows[j].Cells[k++].Value = kvp.Value.strName;


                    if (kvp.Value.x != -1)
                        grid.Rows[j].Cells[k++].Value = kvp.Value.x;
                    else
                        k++;
                    if (kvp.Value.y != -1)
                        grid.Rows[j].Cells[k++].Value = kvp.Value.y;
                    else
                        k++;
                    if (kvp.Value.z != -1)
                        grid.Rows[j].Cells[k++].Value = kvp.Value.z;
                    else
                        k++;
                    if (kvp.Value.u != -1)
                        grid.Rows[j].Cells[k++].Value = kvp.Value.u;
                }
            }
        }

        /// <summary>
        /// 绝对运动
        /// </summary>
        /// <param name="nAxis">轴号</param>
        /// <param name="textBox_tar">目标位置</param>
        /// <param name="textBox_speed">速度</param>
        /// <param name="bPositive">方向 true正  false负</param>
        public static void absMove(int nAxis, TextBox textBox_tar, TextBox textBox_speed, bool bPositive)
        {
            if (textBox_tar.Text != null)
            {
                int nPos = Convert.ToInt32(textBox_tar.Text);
                int nSpeed = Convert.ToInt32(textBox_speed.Text);
                if (nSpeed != 0)
                {
                    MotionMgr.GetInstance().AbsMove(nAxis, nPos/*bPositive ?  nPos : -nPos*/, nSpeed);
                }
                else
                {
                    MessageBox.Show("轴速度设置为0");
                }
            }
            else
            {
                MessageBox.Show("目标位置设置为空");
            }
        }
        
        /// <summary>
        /// 相对运动
        /// </summary>
        /// <param name="nAxis">轴号</param>
        /// <param name="textBox_tar"><目标位置距离/param>
        /// <param name="textBox_speed">速度</param>
        /// <param name="bPositive">方向 true正  false负</param>
        public static void relMove(int nAxis, TextBox textBox_tar, TextBox textBox_speed, bool bPositive)
        {
            if (textBox_tar.Text != null)
            {
                int nPos = Convert.ToInt32(textBox_tar.Text);
                int nSpeed = Convert.ToInt32(textBox_speed.Text);
                if (nSpeed != 0)
                {
                    MotionMgr.GetInstance().RelativeMove(nAxis, bPositive ? nPos : -nPos, nSpeed);
                }
                else
                {
                    MessageBox.Show("轴速度设置为0");
                }
            }
            else
            {
                MessageBox.Show("目标距离设置为空");
            }
        }
        
        /// <summary>
        /// jog运动
        /// </summary>
        /// <param name="nAxis">轴号</param>
        /// <param name="textBox_tar">目标位置</param>
        /// <param name="textBox_speed">速度</param>
        /// <param name="bPositive">方向 true正  false负</param>
        public static void jogMove(int nAxis, TextBox textBox_tar, TextBox textBox_speed, bool bPositive)
        {
            if (textBox_tar.Text != null)
            {
                int nSpeed = Convert.ToInt32(textBox_speed.Text);
                if (nSpeed != 0)
                {
                    MotionMgr.GetInstance().JogMove(nAxis, bPositive, 1, nSpeed);
                }
                else
                {
                    MessageBox.Show("轴速度设置为0");
                }
            }
        }
        
        /// <summary>
        /// 根据选中的点位表格单元单轴移动
        /// </summary>
        /// <param name="dataGridView_point">点位表格</param>
        /// <param name="sta">工站对象</param>
        /// <param name="tb">速度</param>
        public static void singleMove(DataGridView dataGridView_point, StationBase sta, TextBox[] tb)
        {
            int m = dataGridView_point.CurrentCell.ColumnIndex - 2;
            if (m < 0) return;
            if (dataGridView_point.CurrentCell.Value != null)
            {
                int nPos = Convert.ToInt32(dataGridView_point.CurrentCell.Value.ToString());
                int nAxis = 0;
                if (m == 0)
                    nAxis = sta.AxisX;
                else if (m == 1)
                    nAxis = sta.AxisY;
                else if (m == 2)
                    nAxis = sta.AxisZ;
                if (m == 3)
                    nAxis = sta.AxisU;
                if(tb[m] != null)
                {
                    int nSpeed = Convert.ToInt32(tb[m].Text);
                    if (nSpeed > 0)
                        MotionMgr.GetInstance().AbsMove(nAxis, nPos, nSpeed);
                    else
                        MessageBox.Show("当前轴速度设置为0");
                }
               
            }
        }

        /// <summary>
        /// 根据选中的点位表格行全轴移动
        /// </summary>
        /// <param name="dataGridView_point">点位表格</param>
        /// <param name="sta">工站对象</param>
        /// <param name="tb">速度</param>
        public static void allMove(DataGridView dataGridView_point, StationBase sta, TextBox[] tb)
        {
            int n = dataGridView_point.CurrentCell.RowIndex;

            for(int i=0; i<4; ++i)
            {
                if(dataGridView_point.Rows[n].Cells[i+2].Value != null)
                {
                    int nPos = Convert.ToInt32(dataGridView_point.Rows[n].Cells[i + 2].Value.ToString());
                    int nAxis = 0;
                    if (i == 0)
                        nAxis = sta.AxisX;
                    else if (i == 1)
                        nAxis = sta.AxisY;
                    else if (i == 2)
                        nAxis = sta.AxisZ;
                    if (i == 3)
                        nAxis = sta.AxisU;

                    if (tb[i] != null)
                    {
                        int nSpeed = Convert.ToInt32(tb[i].Text);
                        if (nSpeed > 0)
                            MotionMgr.GetInstance().AbsMove(nAxis, nPos, nSpeed);
                        else
                            MessageBox.Show("当前轴速度设置为0");
                    }
                }
            }
        }

        /// <summary>
        /// 根据选中的点位表格更新此轴的位置
        /// </summary>
        /// <param name="dataGridView_point">点位表格</param>
        /// <param name="sta">工站对象</param>
        public static void updateAxisPos(DataGridView dataGridView_point, StationBase sta)
        {
            int m = dataGridView_point.CurrentCell.ColumnIndex - 2;
            if (m >= 0)//zd
            {
                if (dataGridView_point.CurrentCell.Value != null)
                {
                    int nAxis = 0;
                    if (m == 0)
                        nAxis = sta.AxisX;
                    else if (m == 1)
                        nAxis = sta.AxisY;
                    else if (m == 2)
                        nAxis = sta.AxisZ;
                    if (m == 3)
                        nAxis = sta.AxisU;
                    long nPos = MotionMgr.GetInstance().GetAixsPos(nAxis);
                    dataGridView_point.CurrentCell.Value = nPos.ToString();
                }
            }
        }
        
        /// <summary>
        /// 根据选中的点位表格更新此点所有轴的位置
        /// </summary>
        /// <param name="dataGridView_point">点位表格</param>
        /// <param name="sta">工站对象</param>
        public static void updatePointPos(DataGridView dataGridView_point, StationBase sta)
        {
            int m = dataGridView_point.CurrentCell.RowIndex;
            for(int i=0; i<4; ++i)
            {
                if(dataGridView_point.Rows[m].Cells[i+2].Value != null)
                {
                    int nAxis = 0;
                    if (i == 0)
                        nAxis = sta.AxisX;
                    else if (i == 1)
                        nAxis = sta.AxisY;
                    else if (i == 2)
                        nAxis = sta.AxisZ;
                    if (i == 3)
                        nAxis = sta.AxisU;

                    dataGridView_point.Rows[m].Cells[i + 2].Value = MotionMgr.GetInstance().GetAixsPos(nAxis).ToString();
                }
            }
        }

        /// <summary>
        /// 保存当前工站所有点位到文件
        /// </summary>
        /// <param name="dataGridView_point">点位表格</param>
        /// <param name="sta">工站对象</param>
        public static void SavePoint(DataGridView dataGridView_point, StationBase sta)
        {
            Dictionary<int, PointInfo> pp = sta.m_dicPoint;
          //  pp.Clear();
            for (int i = 0; i < dataGridView_point.RowCount; ++i)
            {               
                if (dataGridView_point.Rows[i].Cells[0].Value != null)
                {
                    //index
                    int n = Convert.ToInt32(dataGridView_point.Rows[i].Cells[0].Value.ToString());
                    //PointPos point = pp[n];
                    PointInfo point = new PointInfo();
                    if (dataGridView_point.Rows[i].Cells[1].Value != null)
                        point.strName = dataGridView_point.Rows[i].Cells[1].Value.ToString();
                    else
                        point.strName = string.Empty;

                    if (dataGridView_point.Rows[i].Cells[2].Value != null)
                        point.x = Convert.ToInt32(dataGridView_point.Rows[i].Cells[2].Value.ToString());
                    else
                        point.x = -1;
                    if (dataGridView_point.Rows[i].Cells[3].Value != null)
                        point.y = Convert.ToInt32(dataGridView_point.Rows[i].Cells[3].Value.ToString());
                    else
                        point.y = -1;

                    if (dataGridView_point.Rows[i].Cells[4].Value != null)
                        point.z = Convert.ToInt32(dataGridView_point.Rows[i].Cells[4].Value.ToString());
                    else
                        point.z = -1;

                    if (dataGridView_point.Rows[i].Cells[5].Value != null)
                        point.u = Convert.ToInt32(dataGridView_point.Rows[i].Cells[5].Value.ToString());
                    else
                        point.u = -1;
                    
                    sta.m_dicPoint.Remove(n);
                    sta.m_dicPoint.Add(n, point);//zd                    
                }
                 StationMgr.GetInstance().SavePointFile();
            }

        }

        /// <summary>
        /// 通过窗体对象(工站对话框)更新轴状态
        /// </summary>
        /// <param name="frm">窗体对象</param>
        /// <param name="textBox_pos"></param>
        /// <param name="label_state"></param>
        public static void UpdateMotionState(Form frm, TextBox[]textBox_pos, Label[,]label_state)
        {
            StationBase sta = StationMgr.GetInstance().GetStation(frm);
            if (sta != null)
            {
                for (int i = 0; i < 4; ++i)
                {
                    int nAxis = sta.GetAxisNo(i);
                    if (nAxis > 0)
                    {
                        textBox_pos[i].Text = MotionMgr.GetInstance().GetAixsPos(nAxis).ToString();
                        int nstate = (int)MotionMgr.GetInstance().GetMotionIoState(nAxis);
               //         if(nstate > 0)
                        {
                            for (int j = 0; j < 8; ++j)
                            {
                                int n = (nstate & (0x1 << j)) > 0 ? 1 : 0;
                                if (label_state[i, j + 1].ImageIndex != n)
                                {
                                    label_state[i, j + 1].ImageIndex = n;
                                }
                            }
                        }

                    }

                }
            }
        }

        /// <summary>
        /// 通过轴号更新相应轴状态
        /// </summary>
        /// <param name="nAxisNo">轴号</param>
        /// <param name="textBox_pos"></param>
        /// <param name="label_state"></param>
        public static void UpdateMotionState(int nAxisNo, TextBox textBox_pos, Label[] label_state)
        {

                textBox_pos.Text = MotionMgr.GetInstance().GetAixsPos(nAxisNo).ToString();
                int nstate = (int)MotionMgr.GetInstance().GetMotionIoState(nAxisNo);
                for (int i = 0; i < 8; ++i)
                {
                    if (label_state[i].ImageIndex != (nstate & (0x1 << i)))
                    {
                        label_state[i].ImageIndex = (nstate & (0x1 << i));
                    }
                }
           
        }

        /// <summary>
        /// 显示系统IO输入输出点名字
        /// </summary>
        /// <param name="btn">按钮对象数组</param>
        /// <param name="strIO">要显示IO点名字索引数组</param>
        /// <param name="bIn">ture为输入,false为输出</param>
        public static void updateIoText(Button[] btn, string[] strIO, bool bIn = true)
        {
            for (int i = 0; i < btn.Length; ++i)
            {
                if (i > strIO.Length-1)
                {
                    btn[i].Visible = false;
                }
                else
                {
                    string[] str = strIO[i].Split('.');
                    if (str.Length == 2)
                    {
                        int nCardNo = Convert.ToInt32(str[0]) - 1;
                        int nIndex = Convert.ToInt32(str[1]) - 1;
                        if (nCardNo < IoMgr.GetInstance().CountCard)
                        {
                            if (bIn)
                                btn[i].Text = string.Format("{0}.{1,2} {2}", str[0], str[1],
                                            IoMgr.GetInstance().m_listCard.ElementAt(nCardNo).m_strArrayIn[nIndex]);
                            else
                                btn[i].Text = string.Format("{0}.{1,2} {2}", str[0], str[1],
                                        IoMgr.GetInstance().m_listCard.ElementAt(nCardNo).m_strArrayOut[nIndex]);
                            btn[i].Visible = true;
                        }
                    }
                    else
                        btn[i].Visible = false;
                }
            }
        }

        /// <summary>
        /// 更新IO的状态
        /// </summary>
        /// <param name="btn">按钮对象数组</param>
        /// <param name="strIO">字符串数组,包含卡号和位数</param>
        /// <param name="bIn">ture为输入,false为输出</param>
        public static void updateIoState(Button[] btn, string[] strIO, bool bIn = true)
        {
            for (int i = 0; i < strIO.Length; ++i)
            {
                if (i > btn.Length - 1)
                    break;
                string[] str = strIO[i].Split('.');
                if (str.Length == 2)
                {
                    int ncard = Convert.ToInt32(str[0]);
                    int nIndex = Convert.ToInt32(str[1]);
                    if (ncard < IoMgr.GetInstance().CountCard + 1)
                    {
                        bool bBit = bIn ? IoMgr.GetInstance().ReadIoInBit(ncard, nIndex)
                                : IoMgr.GetInstance().ReadIoOutBit(ncard, nIndex);
                        if (btn[i].ImageIndex != Convert.ToInt32(bBit))
                        {
                            btn[i].ImageIndex = Convert.ToInt32(bBit);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 单击输出按钮事件
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">附带数据的对象</param>
        public static void Form_IO_Out_Click(object sender, EventArgs e)
        {
            string str = ((Button)sender).Text;
            Regex rg = new Regex(@"[0-9]+");  //正则表达式

            Match m = rg.Match(str);

            if (m.Length == 0)
                return;

            int nCard = Convert.ToInt32(m.Value);
            int nIndex = Convert.ToInt32(m.NextMatch().Value);
            if (nCard > 0 && (nCard - 1 < IoMgr.GetInstance().CountCard))
            {
                if (nIndex > 0 && (nIndex - 1 < IoMgr.GetInstance().m_listCard.ElementAt(nCard-1).m_strArrayOut.Length))
                {

                    IoMgr.GetInstance().WriteIoBit(nCard, nIndex, !IoMgr.GetInstance().ReadIoOutBit(nCard, nIndex));
                }
            }
        }
    }
}
