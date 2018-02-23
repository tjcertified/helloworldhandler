using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for HelloWorldModule
/// </summary>
public class HelloWorldModule : IHttpModule
{
    private List<long> contentSizes;

    public HelloWorldModule()
    {
        //
        // TODO: Add constructor logic here
        //
        contentSizes = new List<long>();
    }

    public String ModuleName
    {
        get { return "HelloWorldModule"; }
    }

    public void Dispose()
    {
    }

    public void Init(HttpApplication application)
    {
        application.BeginRequest += Application_BeginRequest;
        application.EndRequest += Application_EndRequest;
        application.PreRequestHandlerExecute += Application_PreRequestHandlerExecute;
        application.PostRequestHandlerExecute += Application_PostRequestHandlerExecute;
    }

    private void Application_PreRequestHandlerExecute(object sender, EventArgs e)
    {
        HttpContext.Current.Items[HANDLER_START_KEY] = DateTime.Now;
    }

    private void Application_PostRequestHandlerExecute(object sender, EventArgs e)
    {
        HttpContext.Current.Items[HANDLER_END_KEY] = DateTime.Now;
    }

    private void Application_EndRequest(object sender, EventArgs e)
    {
        // This means that the request did not go completely through the pipeline for some reason
        // (css file, redirect, image, etc.), so it is not necessarily going to have all of the
        // information that we need to process it.
        if (HttpContext.Current.Items[HANDLER_END_KEY] == null)
        {
            return;
        }
        var appEnd = DateTime.Now;
        DateTime appStart = (DateTime)HttpContext.Current.Items[APP_START_KEY];
        DateTime handlerStart = (DateTime)HttpContext.Current.Items[HANDLER_START_KEY];
        DateTime handlerEnd = (DateTime)HttpContext.Current.Items[HANDLER_END_KEY];
        contentSizes.Add(_watcher.Length);
        HttpContext.Current.Response.Write("<hr><h1><font color=red> HelloWorldModule: Begin Request </font></h1>"
            + "<p>Response Size: " + _watcher.Length + " bytes.</p>"
            + "<p>Total Pipeline Requests So Far: " + contentSizes.Count + "</p>"
            + "<p>Total Request Time: " + appEnd.Subtract(appStart).TotalMilliseconds + "ms.</p>"
            + "<p>Total HttpHandler Time: " + handlerEnd.Subtract(handlerStart).TotalMilliseconds + "ms.</p>"
            + "<p>Average response size: " + contentSizes.Average() + " bytes.</p>"
            + "<p>Largest response size: " + contentSizes.Max() + " bytes.</p>"
            + "<p>Smallest response size: " + contentSizes.Min() + " bytes.</p>"
            + "<hr>");
    }

    private void Application_BeginRequest(object sender, EventArgs e)
    {
        HttpContext.Current.Items[APP_START_KEY] = DateTime.Now;
        _watcher = new StreamWatcher(HttpContext.Current.Response.Filter);
        HttpContext.Current.Response.Filter = _watcher;
    }

    private const string APP_START_KEY = "appStartTime";
    private const string APP_END_KEY = "appEndTime";
    private const string HANDLER_START_KEY = "handlerStartTime";
    private const string HANDLER_END_KEY = "handlerEndTime";
    private StreamWatcher _watcher;
}