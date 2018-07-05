using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Globalization;

namespace WindowsFormsApplication6 {
    /// <summary>
    /// 弹出窗口
    /// </summary>
    sealed partial class PopuForm : Form {
        string _sender = string.Empty;
        DateTime _date = DateTime.Now;
        string _content = string.Empty;

        private WebBrowser txtMessage;
        /// <summary>
        /// 设置新的消息，更新所有消息的list
        /// </summary>
        public void SetNewMessages(string sender,DateTime date,string content) {
            this._sender = sender;
            this._date = date;
            this._content = content;
        }

        /// <summary>
        /// 弹出窗口
        /// </summary>
        /// <param name="messages"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public PopuForm(string sender, DateTime date, string content)
            : base() {
            InitializeComponent();

            this._sender = sender;
            this._date = date;
            this._content = content;

        }

        //下面是可用的常量，根据不同的动画效果声明自己需要的
        private const int AW_HOR_POSITIVE = 0x0001;//自左向右显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志
        private const int AW_HOR_NEGATIVE = 0x0002;//自右向左显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志
        private const int AW_VER_POSITIVE = 0x0004;//自顶向下显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志
        private const int AW_VER_NEGATIVE = 0x0008;//自下向上显示窗口，该标志可以在滚动动画和滑动动画中使用。使用AW_CENTER标志时忽略该标志该标志
        private const int AW_CENTER = 0x0010;//若使用了AW_HIDE标志，则使窗口向内重叠；否则向外扩展
        private const int AW_HIDE = 0x10000;//隐藏窗口
        private const int AW_ACTIVE = 0x20000;//激活窗口，在使用了AW_HIDE标志后不要使用这个标志
        private const int AW_SLIDE = 0x40000;//使用滑动类型动画效果，默认为滚动动画类型，当使用AW_CENTER标志时，这个标志就被忽略
        private const int AW_BLEND = 0x80000;//使用淡入淡出效果

        //窗体代码（将窗体的FormBorderStyle属性设置为none）：
        private void PopupForm_Load(object sender, EventArgs e) {
            int x = Screen.PrimaryScreen.WorkingArea.Right - this.Width;
            int y = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height;
            this.Location = new Point(x, y);//设置窗体在屏幕右下角显示
            NativeMethods.AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_ACTIVE | AW_VER_NEGATIVE);
            ShowMessages();
        }

        private void PopupForm_Closing(object sender, FormClosingEventArgs e) {
            NativeMethods.AnimateWindow(this.Handle, 1000, AW_BLEND | AW_HIDE);
        }

        /// <summary>
        /// 将消息显示出来
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public void ShowMessages() {

            this.txtMessage.Navigate("about:blank");
            while (this.txtMessage.ReadyState != WebBrowserReadyState.Complete) {
                Application.DoEvents();
            }

            StringBuilder result = new StringBuilder();
            result.Append("<HTML>");
            result.AppendFormat("<span style=\"font-family:黑体; font-size:10pt;color:Blue;\">{0}</span> ", _sender);
            result.AppendFormat("<span style=\"font-family:黑体; font-size:8pt;color :#ff8c00;\">{0}</span>", _date);
            result.Append("<br />");
            result.Append(GetContentElementString());
            result.Append("<br />");
            result.Append("<span style=\"font-size:8pt;color:Green;\">----------------------------------------------------------------</span>");
            result.Append("<br />");
            result.Append("</HTML>");
            txtMessage.Document.Write(result.ToString());
        }

        private string GetContentElementString() {
            return string.Format(CultureInfo.CurrentCulture, "<span style=\"font-family:宋体; font-size:8pt;color:Black;\">{0}</span>", _content);
        }

        private void InitializeComponent() {
            this.txtMessage = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // txtMessage
            // 
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessage.Location = new System.Drawing.Point(0, 0);
            this.txtMessage.MinimumSize = new System.Drawing.Size(20, 20);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(339, 147);
            this.txtMessage.TabIndex = 0;
            // 
            // InformalMessagePopupForm
            // 
            this.ClientSize = new System.Drawing.Size(339, 147);
            this.Controls.Add(this.txtMessage);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InformalMessagePopupForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "最新消息";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.PopupForm_Load);
            this.ResumeLayout(false);

        }

    }

    internal static class NativeMethods {
        /// 窗体动画函数
        /// </summary>
        /// <param name="hwnd">指定产生动画的窗口的句柄</param>
        /// <param name="dwTime">指定动画持续的时间</param>
        /// <param name="dwFlags">指定动画类型，可以是一个或多个标志的组合。</param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);

    }
}
