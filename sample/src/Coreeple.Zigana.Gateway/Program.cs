using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddZigana(builder.Configuration)
    .UseNpgsql();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//app.MapZiganaApi();

app.UseZigana();

DiagnosticListener.AllListeners.Subscribe(new Subscriber());

app.Run();






class MyLibraryListener : IObserver<KeyValuePair<string, object?>>
{
    public void OnCompleted() { }
    public void OnError(Exception error) { }

    public void OnNext(KeyValuePair<string, object?> keyValue)
    {
        switch (keyValue.Key)
        {
            case "HttpRequestHandler.Start":
                Console.WriteLine($"HttpRequestHandler.Start - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId}");
                break;
            case "HttpRequestHandler.Stop":
                if (Activity.Current?.Events != null)
                {
                    foreach (var item in Activity.Current.Events)
                    {
                        Console.WriteLine(item.Tags.FirstOrDefault());
                    }
                }

                Console.WriteLine($"HttpRequestHandler.Stop - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId}");
                break;
            case "HttpRequestActionExecutor.Start":
                Console.WriteLine($"HttpRequestActionExecutor.Start - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId}");
                break;
            case "HttpRequestActionExecutor.Stop":
                Console.WriteLine($"HttpRequestActionExecutor.Stop - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId}");
                break;
            case "ActionExecuteManager.Start":
                Console.WriteLine($"ActionExecuteManager.Start - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId}");
                break;
            case "ActionExecuteManager.Stop":
                Console.WriteLine($"ActionExecuteManager.Stop - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId}");
                break;
            case "HtmlParserActionExecutor.Start":
                Console.WriteLine($"HtmlParserActionExecutor.Start - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId}");
                break;
            case "HtmlParserActionExecutor.Stop":
                Console.WriteLine($"HtmlParserActionExecutor.Stop - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId}");
                break;
            default:
                break;
        }
    }
}

class Subscriber : IObserver<DiagnosticListener>
{
    public void OnCompleted() { }
    public void OnError(Exception error) { }

    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "Coreeple.Zigana")
        {
            listener.Subscribe(new MyLibraryListener());
        }
    }
}