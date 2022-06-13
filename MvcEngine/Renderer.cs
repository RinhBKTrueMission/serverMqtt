using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Mvc
{
    public abstract class Renderer<TView, TModel> : IView
        where TView : new()
    {
        public ViewDataDictionary ViewBag { get; set; }
        public TModel Model { get; set; }

        protected TView _mainContent;
        public object Content => _mainContent;
        public void Render(object model)
        {
            Model = (TModel)model;
            _mainContent = CreateContent();
            
            RenderCore();
        }
        protected abstract void RenderCore();
        protected virtual TView CreateContent()
        {
            return new TView();
        }
    }
}
