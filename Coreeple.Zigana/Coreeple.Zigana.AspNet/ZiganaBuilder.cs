﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Coreeple.Zigana.AspNet;
public class ZiganaBuilder(IServiceCollection services, IConfiguration configuration)
{
    public IServiceCollection Services { get; } = services;
    public IConfiguration Configuration { get; } = configuration;
}