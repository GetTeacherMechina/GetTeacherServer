using GetTeacherServer.Services.Generators;

namespace GetTeacherServer.BuilderExtensions;

public static class GetTeacherBuilderExtensions
{ 
    public static void UseGetTeacherServices(this WebApplicationBuilder builder)
    {
        builder.UseGetTeacherDb(); 
        builder.Services.AddScoped<JwtTokenGenerator>();
    }
}