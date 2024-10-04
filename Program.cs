using ConsoleApp3;
using RestApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/GetImageTags", async (HttpContext context, string imageFileLink) =>
{
  try
  {
      // string imageFileLink = (string)context.Request.RouteValues["imageFileLink"];
      List<string> tags = await Tagging.PictureTaggingAsync(imageFileLink);

       ResultModel<List<string>> result = new ResultModel<List<string>>();
       result.Data = tags;
       result.IsSuccess = true;
       return Results.Ok(result);
  }
  catch (System.Exception ex)
  {
      ResultModel<string> result = new ResultModel<string>(); 
       result.IsSuccess = false;
       result.Message = ex.Message;
       return Results.Ok(result);
  }
});



app.MapGet("/GetTextWithOCR", async (HttpContext context, string imageUrl) =>
{
  try
  {
      //public static async Task<StringBuilder> ExtractImageTextWithOCR(byte[] imageData)
      string text = await HukukPortal.ExtractImageTextWithOCR(imageUrl);

       ResultModel<string> result = new ResultModel<string>();
       result.Data = text;
       result.IsSuccess = true;
       return Results.Ok(result);
  }
  catch (System.Exception ex)
  {
      ResultModel<string> result = new ResultModel<string>(); 
       result.IsSuccess = false;
       result.Message = ex.Message;
       return Results.Ok(result);
  }
    
 
});

app.MapGet("/DocumentSummary", async (HttpContext context, string docuemtnContent) =>
{
  try
  {
      string summary = await HukukPortal.documentSummary(docuemtnContent);

       ResultModel<string> result = new ResultModel<string>();
       result.Data = summary;
       result.IsSuccess = true;
       return Results.Ok(result);
  }
  catch (System.Exception ex)
  {
      ResultModel<string> result = new ResultModel<string>(); 
       result.IsSuccess = false;
       result.Message = ex.Message;
       return Results.Ok(result);
  }
});

 
app.RegisterImageTagsEndpoints();
app.Run();
 
//dotnet watch -lp https ile çalıştırıldı