using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CommonTool;

using System.Threading;

using HalconDotNet;

namespace AutoFrameVision
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Form_Vision_debug : Form, IVisionControlUpdate
    {
        HObject m_image = null;      //当前图像缓存，只用于文件读取
        bool m_bRunThread = false;   //线程是否在运行中
        Thread m_thread = null;     //连续采集的线程       
        CameraBase m_camera = null; //当前选择的相机采集实例
        bool m_bPause = false;      //是否在暂停中
        bool m_bAutoTest = false;   //是否开启自动测试
        bool m_bBatch = false;      //是否在批量测试过程中

         /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <returns></returns>
        public Form_Vision_debug()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化时根据视觉管理器配置添加各相机及步骤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Vision_debug_Load(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, CameraBase> cb in VisionMgr.GetInstance().m_dicCamera)
            {
                comboBox_cam.Items.Add(cb.Key);
            }
            comboBox_cam.SelectedIndex = 0;

            foreach (KeyValuePair<string, VisionBase> vb in VisionMgr.GetInstance().m_dicVision)
            {
                comboBox_step.Items.Add(vb.Key);
            }
            comboBox_step.SelectedIndex = 0;

            HOperatorSet.GenEmptyObj(out m_image);
            //        textBox_dir.Text = SystemMgr.GetInstance().GetImagePath();
            this.BeginInvoke((MethodInvoker)delegate
            {
                visionControl1.InitWindow();
            });
            
            //增加权限等级变更通知
            OnModeChanged();
            Security.ModeChangedEvent += OnModeChanged;
        }


        /// <summary>
        /// 权限变更响应
        /// </summary>
        private void OnModeChanged()
        {
            if (Security.IsEngMode())
            {
                groupBox_cam.Enabled = true;
                groupBox_func.Enabled = true;
                button_cali.Enabled = true;
            }
            else
            {
                groupBox_cam.Enabled = false;
                groupBox_func.Enabled = false;
                button_cali.Enabled = false;
            }
        }
        /// <summary>
        /// 在视觉显示控件要求更新时,用当前内容更新它
        /// </summary>
        /// <param name="ctl"></param>
        public void UpdateVisionControl(VisionControl ctl)
        {
            HTuple num = 0;
            ctl.LockDisplay();
            try
            {
                if (m_image != null && m_image.IsInitialized() && m_image.Key != IntPtr.Zero)
                {
                    HOperatorSet.DispImage(m_image, ctl.GetHalconWindow());

                }

            }
            catch (HalconException HDevExpDefaultException1)
            {
                System.Diagnostics.Debug.WriteLine(HDevExpDefaultException1.ToString());

            }
            catch (AccessViolationException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());

            }
            finally
            {
                ctl.UnlockDisplay();

            }
        }


        /// <summary>
        /// 捕捉一张图像并显示
        /// </summary>  
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_snap_Click(object sender, EventArgs e)
        {
            if (m_thread != null)
            {
                StopThread();
            }
            HObject img = VisionMgr.GetInstance().CameraSnap(comboBox_cam.Text);
            if (img != null)
            {
                m_image = img;
                if(m_bAutoTest)
                {
                    TestImage();
                }
                else
                {
                    visionControl1.RegisterUpdateInterface(this);
                    Action<object> action = (object obj) =>
                    {
                        visionControl1.DispImageFull(m_image);
                    };
                    Task t1 = new Task(action, "");
                    t1.Start();
                    t1.Wait();
                }
            }
        }

        /// <summary>
        /// 异步采集一幅图像并显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_grab_Click(object sender, EventArgs e)
        {
            StopThread();

            m_camera = VisionMgr.GetInstance().GetCam(comboBox_cam.Text);
            if (m_camera != null)
            {              
                button_grab.Enabled = false;
                button_batch_test.Enabled = false;
                button_stop_grab.Enabled = true;
                StartThread();
            }
        }

        /// <summary>
        /// 异步采集线程及批量处理线程
        /// </summary>
        void ThreadGrab()
        {
            if (m_camera != null && m_camera.Open())
            {
                while (m_bRunThread)
                {
                    if(m_bPause == false)
                    {
                        visionControl1.LockDisplay();
                        int n = m_camera.Grab();
                        visionControl1.UnlockDisplay();
                        if (n != 0)
                        {
                          
                            m_image = m_camera.GetImage();
                         
                            if (m_bAutoTest || m_bBatch)
                            {
                                TestImage();
                            }
                            else
                            {
                                visionControl1.RegisterUpdateInterface(this);
                                UpdateVisionControl(visionControl1);

                            }

                            if (n == -1)//采集完成,自动停止
                            {
                                this.BeginInvoke((MethodInvoker)delegate
                                {
                                    this.button_batch_stop.PerformClick();
                                });
                                break;
                            }                              
                       //     visionControl1.DispImageFull(m_image);
                        } 
                                               
                    }
                    if(m_bBatch )
                    {
                        int n = 100;
                        this.Invoke((MethodInvoker)delegate
                        {
                            n = Convert.ToInt32(textBox_span.Text);
                        });
                        Thread.Sleep(n);
                    }
                    else
                    {
                        Thread.Sleep(20);
                    }                    
                }
            }
            if (m_camera != null)
            {
                m_camera.StopGrab();
                m_camera = null;
            }
            return;
        }
        /// <summary>
        /// 开始异步采集或批量处理
        /// </summary>
        void StartThread()
        {
            if (m_thread == null)
            {
                m_thread = new Thread(ThreadGrab);
                m_bRunThread = true;
                m_thread.Start();
            }
            if (m_thread.ThreadState != ThreadState.Running)
            {               
                m_thread.Start();
            }
        }

        /// <summary>
        /// 停止线程
        /// </summary>
        void StopThread()
        {
            button_stop_grab.Enabled = false;
            button_batch_stop.Enabled = false;
            button_batch_pause.Enabled = false;

            button_batch_test.Enabled = true;
            button_grab.Enabled = true;
            m_bBatch = false;

            if (m_thread != null)
            {
                m_bRunThread = false;
                if (m_thread.Join(5000) == false)
                    m_thread.Abort();
                
                m_thread = null;
            }
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_stop_Click(object sender, EventArgs e)
        {
            StopThread();
        }

        /// <summary>
        /// 打开图像文件并显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_open_Click(object sender, EventArgs e)
        {          
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StopThread();
                HOperatorSet.ReadImage(out m_image, openFileDialog1.FileName);
                if (m_bAutoTest)
                {
                    TestImage();
                }
                else
                {
                    visionControl1.RegisterUpdateInterface(this);
                    Action<object> action = (object obj) =>
                    {
                        visionControl1.DispImageFull(m_image);
                    };
                    Task t1 = new Task(action, "");
                    t1.Start();
                    t1.Wait();
                }
            }
        }

        /// <summary>
        /// 当选择的相机改变时自动停止采集线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_cam_SelectedIndexChanged(object sender, EventArgs e)
        {
            StopThread();            
        }

        /// <summary>
        /// 窗口关闭时停止线程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form_Vision_debug_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopThread();
            //m_image.Dispose();
        }

        /// <summary>
        /// 保存当前图像至image_tmp目录下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_save_Click(object sender, EventArgs e)
        {
            string strDir = SystemMgr.GetInstance().GetImagePath("\\image_temp");
            string strFile = strDir + "\\" + comboBox_cam.Text + DateTime.Now.ToString(" yyyyMMdd_HH_mm_ss");
            HOperatorSet.WriteImage(m_image, "bmp", 0, strFile);
        }

        /// <summary>
        /// 用当前图像和当前选中的图像步骤测试图像算法并显示
        /// </summary>
        void TestImage()
        {
            if (m_image != null)
            {
             //   try
                {
                    HTuple num = 0;
                    HOperatorSet.CountObj(m_image, out num);
                    if (num > 0)//&& m_image.IsInitialized() && m_image.Key != IntPtr.Zero)
                    {
                        string strStep = string.Empty;
                        this.Invoke((MethodInvoker)delegate
                        {
                            strStep = comboBox_step.Text;
                        });

                        VisionMgr.GetInstance().ProcessImage(strStep, m_image, visionControl1);
                    }
                }
             //   catch { }
            }
        }

        /// <summary>
        /// 测试当前图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_test_Click(object sender, EventArgs e)
        {
            StopThread();
            TestImage();
        }
        /// <summary>
        /// 重新选择批量处理图像的目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_path_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowNewFolderButton = false;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox_dir.Text = folderBrowserDialog1.SelectedPath;
                textBox_dir.SelectionStart = textBox_dir.TextLength;
            }
        }
        /// <summary>
        /// 利用后台线程开始批量测试图像算法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_test_batch_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrEmpty(textBox_dir.Text) == false && Directory.Exists(textBox_dir.Text))
            {
                StopThread();
                m_camera = new CameraFile(textBox_dir.Text);
                if (m_camera != null)
                {
                    button_batch_stop.Enabled = true;
                    button_batch_pause.Enabled = true;
                    button_grab.Enabled = false;
                    button_batch_test.Enabled = false;

                    m_bBatch = true;
                    StartThread();
                }
            }
        }

        /// <summary>
        /// 暂停批量测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_batch_pause_Click(object sender, EventArgs e)
        {
            if(button_batch_pause.Text == "暂停测试")
            {
                m_bPause = true;
                button_batch_pause.Text = "继续测试";
            }
            else
            {
                m_bPause = false;
                button_batch_pause.Text = "暂停测试";
            }
        }

        /// <summary>
        /// 停止批量测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_batch_stop_Click(object sender, EventArgs e)
        {
            StopThread();
        }

        /// <summary>
        /// 切换自动测试模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox_auto_Click(object sender, EventArgs e)
        {
            m_bAutoTest = !m_bAutoTest;
            checkBox_auto.Checked = m_bAutoTest;

        }

        /// <summary>
        /// 显示标定对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_light_Click(object sender, EventArgs e)
        {
            Form_CaliNPoint cnp = new Form_CaliNPoint();
            cnp.ShowDialog(this);
        }

        /// <summary>
        /// 关闭对话框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button_setExposure_Click(object sender, EventArgs e)
        {
            int index = comboBox_step.SelectedIndex;
            if (textBox_exposure.Text == "")
                return;
            int nExp = Convert.ToInt32(textBox_exposure.Text);
            string strStep = comboBox_step.Text;

            VisionMgr.GetInstance().WriteExposureTime(strStep, nExp);
        }

        private void textBox_exposure_KeyPress(object sender, KeyPressEventArgs e)
        {
           if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8)
                e.Handled = true;
        }

        private void comboBox_step_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strStep = comboBox_step.Text;
            string strExp = "";
            VisionMgr.GetInstance().ReadExposureTime(strStep, out strExp);
            textBox_exposure.Text = strExp;

        }
    }
}
