using Mango.Web;
using Mango.Web.Services;
using Mango.Web.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<IProductService, ProductService>(); //<- configures HttpClientFactory on product service
SD.ProductApiBase = builder.Configuration["ServiceUrl:ProductApi"];//<- Assigns product API base url
builder.Services.AddScoped<IProductService, ProductService>();     //<- Configures dep injection for product service


/******************* Security Authentication setup: ******************************/
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
    .AddCookie("Cookies", config => config.ExpireTimeSpan = TimeSpan.FromMinutes(10))
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["ServiceUrl:IdentityApi"];
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClientId = "mango"; //<= comes from SD file in Identity API
        options.ClientSecret = "secretkey";//<= comes from SD file in Identity API
        options.ResponseType = "code";//<= comes from SD file in Identity API
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.Scope.Add("mango"); //<= comes from SD file in Identity API
        options.SaveTokens = true;
    });
/************** end of authentication setup ********************************/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
