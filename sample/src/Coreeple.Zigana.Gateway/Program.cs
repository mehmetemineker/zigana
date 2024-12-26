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
                Console.WriteLine($"HttpRequestHandler.Start - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId} - StartTimeUtc: {Activity.Current?.StartTimeUtc}");
                break;
            case "HttpRequestHandler.Stop":
                if (Activity.Current?.Events != null)
                {
                    foreach (var item in Activity.Current.Events)
                    {
                        Console.WriteLine($"Name: {item.Name} - Tags: {item.Tags.FirstOrDefault()} - Timestamp: {item.Timestamp}");
                    }
                }

                Console.WriteLine($"HttpRequestHandler.Stop - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId} - StartTimeUtc: {Activity.Current?.StartTimeUtc}");
                break;
            case "HttpRequestActionExecutor.Start":
                Console.WriteLine($"HttpRequestActionExecutor.Start - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId} - StartTimeUtc: {Activity.Current?.StartTimeUtc}");
                break;
            case "HttpRequestActionExecutor.Stop":
                Console.WriteLine($"HttpRequestActionExecutor.Stop - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId} - StartTimeUtc: {Activity.Current?.StartTimeUtc}");
                break;
            case "ActionExecuteManager.Start":
                Console.WriteLine($"ActionExecuteManager.Start - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId} - StartTimeUtc: {Activity.Current?.StartTimeUtc}");
                break;
            case "ActionExecuteManager.Stop":
                if (Activity.Current?.Events != null)
                {
                    foreach (var item in Activity.Current.Events)
                    {
                        Console.WriteLine($"Name: {item.Name} - Tags: {item.Tags.FirstOrDefault()} - Timestamp: {item.Timestamp}");
                    }
                }

                Console.WriteLine($"ActionExecuteManager.Stop - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId} - StartTimeUtc: {Activity.Current?.StartTimeUtc}");
                break;
            case "HtmlParserActionExecutor.Start":
                Console.WriteLine($"HtmlParserActionExecutor.Start - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId} - StartTimeUtc: {Activity.Current?.StartTimeUtc}");
                break;
            case "HtmlParserActionExecutor.Stop":
                Console.WriteLine($"HtmlParserActionExecutor.Stop - ActivityId: {Activity.Current?.Id} - ParentId: {Activity.Current?.ParentId} - StartTimeUtc: {Activity.Current?.StartTimeUtc}");
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