﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIAutores.Tests.Mocks
{
    public class URLHelperMock : IUrlHelper
    {
        public ActionContext ActionContext => throw new NotImplementedException();

        public string? Action(UrlActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        [return: NotNullIfNotNull("contentPath")]
        public string? Content(string? contentPath)
        {
            throw new NotImplementedException();
        }

        public bool IsLocalUrl([NotNullWhen(true), StringSyntax("Uri")] string? url)
        {
            throw new NotImplementedException();
        }

        public string? Link(string? routeName, object? values)
        {
            return ""; //No nos importa qué devuelva, por eso le ponemos un string vacío, solo nos importa que no nos de ningún error.
        }

        public string? RouteUrl(UrlRouteContext routeContext)
        {
            throw new NotImplementedException();
        }
    }
}
