using System.Diagnostics;
using System.Net;
using Yarp.ReverseProxy.Forwarder;
using Microsoft.Net.Http.Headers;
using Yarp.ReverseProxy.Transforms;
using System.Net.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpForwarder();

var app = builder.Build();

var socketsHttpHandler = new SocketsHttpHandler()
{
	UseProxy = false,
    AllowAutoRedirect = false,
    AutomaticDecompression = DecompressionMethods.None,
    UseCookies = false,
    ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
    ConnectTimeout = TimeSpan.FromSeconds(15), 
};
var client = new HttpClient(socketsHttpHandler);

var httpClient = new HttpMessageInvoker(socketsHttpHandler);
var transformer = HttpTransformer.Default;
var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };

using (var scope = app.Services.CreateScope())
{
    var forwarder = scope.ServiceProvider.GetRequiredService<IHttpForwarder>();

    app.UseRouting();

    app.Map("/withoutyarp/{**catch-all}", async httpContext =>
    {
        //Console.WriteLine("receiving call - without.start");
        var cid = httpContext.GetRouteValue("cid")?.ToString();

        httpContext.Request.Path = httpContext.Request.Path.ToString().Replace("/withyarp/","/");

        var response = await client.GetStreamAsync("https://restcountries.com/v3.1/alpha/" + httpContext.Request.Path.ToString().Replace("/withoutyarp/","/"));

        await httpContext.Response.WriteAsync("" + new StreamReader(response).ReadToEnd());
        //Console.WriteLine("receiving call - without.end");
    });

    app.Map(
        "/withyarp/{**catch-all}", async httpContext => {
            //Console.WriteLine("receiving call - with.start");
            var cid = httpContext.GetRouteValue("cid")?.ToString();

            httpContext.Request.Path = httpContext.Request.Path.ToString().Replace("/withyarp/", "/");

            var error = await forwarder.SendAsync(httpContext, "https://restcountries.com/v3.1/alpha/",
                httpClient, requestConfig, transformer);
            // Check if the operation was successful
            if (error != ForwarderError.None)
            {
                var errorFeature = httpContext.GetForwarderErrorFeature();
                var exception = errorFeature?.Exception;
            } 
            //Console.WriteLine("receiving call - with.end");
        }
    );
}

app.Run();