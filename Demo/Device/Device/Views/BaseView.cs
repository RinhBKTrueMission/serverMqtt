using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
    
namespace System.Windows.Controls
{
    interface IPopup
    {
        void Show();
    }
    abstract class BaseView<TControl, TModel> : System.Mvc.Renderer<TControl, TModel>
        where TControl : UIElement, new()
    {
        protected void AsyncUpdate(Action func)
        {
            _mainContent.Dispatcher.InvokeAsync(() => func?.Invoke());
        }
    }
    abstract class BaseView<TControl> : BaseView<TControl, JObject>
        where TControl : UIElement, new()
    {
    }
    abstract class TableView<TModel> : BaseView<MyTableLayout, TModel>
    {
    }
    abstract class FormView<TControl> : BaseView<TControl>, IPopup
        where TControl : UIElement, new()
    {
        public virtual void Show()
        {
            var grid = new Grid { 
                Children = { _mainContent }
            };
            var frm = new System.Windows.Window {
                Content = grid
            };
            frm.Show();
        }
    }
}
