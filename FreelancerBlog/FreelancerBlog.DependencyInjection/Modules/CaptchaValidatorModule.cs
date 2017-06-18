﻿using Autofac;
using FreelancerBlog.Core.Services.Shared;
using WebFor.Infrastructure.Services.Shared;

namespace FreelancerBlog.DependencyInjection.Modules
{
    public class CaptchaValidatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CaptchaValidator>().As<ICaptchaValidator>();
        }
    }
}