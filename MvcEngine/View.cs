namespace System.Mvc
{
    public partial interface IView
    {
        ViewDataDictionary ViewBag { get; set; }
        void Render(object model);
        object Content { get; }
    }

    public interface IAsyncView
    {
    }
}
