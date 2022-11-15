using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Corely.UI.Core
{
    public class NotifyBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Change a property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }
            field = value;
            Notify(propertyName);
            return true;
        }

        /// <summary>
        /// Change underlying property value
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="property"></param>
        /// <param name="outExpr"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetProp<T1, T2>(T1 property, Expression<Func<T1, T2>> outExpr, T2 value, string propertyName)
        {
            var expr = (MemberExpression)outExpr.Body;
            var prop = (PropertyInfo)expr.Member;
            if (EqualityComparer<T2>.Default.Equals((T2)prop.GetValue(property, null), value))
            {
                return false;
            }
            prop.SetValue(property, value, null);
            Notify(propertyName);
            return true;
        }

        /// <summary>
        /// Notify property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
