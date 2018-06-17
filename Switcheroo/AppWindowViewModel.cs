using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Switcheroo.Core;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Reflection;

namespace Switcheroo
{
    public class AppWindowViewModel : INotifyPropertyChanged, IWindowText
    {
        [DllImport("TestDll.dll", EntryPoint = "getcmd")]
        public static extern IntPtr getcmd(int i);
        public AppWindowViewModel(AppWindow appWindow)
        {
            AppWindow = appWindow;
        }

        private static String Get1CTitle(Process process, String Title)
        {
            string cmd = Marshal.PtrToStringAuto(getcmd(process.Id));

            var indexIBName = cmd.IndexOf("/IBName");
            var windowTitle = "";
            if (indexIBName != -1)
            {
                var indexEnd = cmd.IndexOf(" /", indexIBName + 7);
                var name = "";

                if (indexEnd != -1)
                {
                    name = cmd.Substring(indexIBName + 7, indexEnd - (indexIBName + 7)).Replace("\"", "");
                }
                else
                {
                    name = cmd.Substring(indexIBName).Replace("\"", "");
                }

                windowTitle = name;

            }
            else
            {
                windowTitle = Title;
            }
            return windowTitle;

        }

        public AppWindow AppWindow { get; private set; }

        #region IWindowText Members

        public string WindowTitle
        {
            get { return Get1CTitle(AppWindow.Process, AppWindow.Title); }
        }

        public string ProcessTitle
        {
            get { return AppWindow.ProcessTitle; }
        }

        #endregion

        #region Bindable properties

        public IntPtr HWnd
        {
            get { return AppWindow.HWnd; }
        }

        private string _formattedTitle;

        public string FormattedTitle
        {
            get { return _formattedTitle; }
            set
            {
                _formattedTitle = value;
                NotifyOfPropertyChange(() => FormattedTitle);
            }
        }

        private string _formattedProcessTitle;

        public string FormattedProcessTitle
        {
            get { return _formattedProcessTitle; }
            set
            {
                _formattedProcessTitle = value;
                NotifyOfPropertyChange(() => FormattedProcessTitle);
            }
        }

        private bool _isBeingClosed = false;

        public bool IsBeingClosed
        {
            get { return _isBeingClosed; }
            set
            {
                _isBeingClosed = value;
                NotifyOfPropertyChange(() => IsBeingClosed);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyOfPropertyChange<T>(Expression<Func<T>> property)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(GetPropertyName(property)));
        }

        private string GetPropertyName<T>(Expression<Func<T>> property)
        {
            var lambda = (LambdaExpression) property;

            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression) lambda.Body;
                memberExpression = (MemberExpression) unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression) lambda.Body;
            }

            return memberExpression.Member.Name;
        }

        #endregion
    }
}