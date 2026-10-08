using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using KelimeEzberApp;
using KelimeEzberApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register Custom App Services
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IGamificationService, GamificationService>();
builder.Services.AddScoped<IWordService, WordService>();

var host = builder.Build();

// Pre-initialize gamification and word services
var gamificationService = host.Services.GetRequiredService<IGamificationService>();
await gamificationService.InitializeAsync();

var wordService = host.Services.GetRequiredService<IWordService>();
await wordService.InitializeAsync();

await host.RunAsync();
