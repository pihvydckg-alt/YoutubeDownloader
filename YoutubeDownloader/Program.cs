using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();
app.UseCors();

var youtube = new YoutubeClient();

app.MapGet("/", () => "API is online!");

app.MapGet("/api/download", async (string url) => {
    try {
        var video = await youtube.Videos.GetAsync(url);
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(url);
        
        var muxed = streamManifest.GetMuxedStreams().OrderByDescending(s => s.VideoQuality).FirstOrDefault();
        var audio = streamManifest.GetAudioOnlyStreams().OrderByDescending(s => s.Bitrate).FirstOrDefault();

        return Results.Ok(new {
            title = video.Title,
            thumbnail = video.Thumbnails.LastOrDefault()?.Url,
            duration = video.Duration?.ToString(),
            videoUrl = muxed?.Url,
            audioUrl = audio?.Url
        });
    } catch (Exception ex) {
        return Results.BadRequest(new { error = ex.Message });
    }
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");
