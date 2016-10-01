using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Core.Mail
{
    public interface ITemplateRenderer
    {
        string Parse<T>(string template,T model);
    }
    public class NustacheTemplateRenderer : ITemplateRenderer
    {
        public string Parse<T>(string template, T model)
        {
            if (model == null)
            {
                return template;
            }
            else
            {
                return Nustache.Core.Render.StringToString(template, model);
            }
        }
    }
}
