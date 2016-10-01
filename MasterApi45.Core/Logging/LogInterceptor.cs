using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Core.Logging
{
    public class LogInterceptor : IInterceptor
    {
        #region Private Members
        private ILogger _logger;
        #endregion

        #region Constructor
        public LogInterceptor(ILogger logger)
        {
            _logger = logger;
        }
        #endregion

        #region Public Methods
        public void Intercept(IInvocation invocation)
        {
            if (_logger.IsInfoEnabled)
            {
                _logger.Info(CreateInvocationLogString("CALLED", invocation));
            }
            else if (_logger.IsDebugEnabled)
            {
                _logger.Debug(string.Format("Entring: {0}.{1}", invocation.TargetType.FullName, invocation.Method.Name));
            }

            try
            {
                //' Call method
                invocation.Proceed();

                //' Save return value
                if (_logger.IsInfoEnabled)
                {
                    var returnType = invocation.Method.ReturnType;
                    if (returnType != typeof(void))
                    {
                        var returnValue = invocation.ReturnValue;

                        if (returnType == typeof(Task))
                        {
                            _logger.Info(CreateInvocationLogString("RETURNING TASK", invocation));
                        }
                        else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                        {
                            _logger.Info(CreateInvocationLogString("RETURNING TASK<>", invocation));

                            var task = (Task)returnValue;
                            task.ContinueWith(antecedent =>
                            {
                                var taskDescriptor = CreateInvocationLogString("Task from", invocation);
                                var result = antecedent.GetType().GetProperty("Result").GetValue(antecedent, null);
                                _logger.Info("Returning for :" + invocation.TargetType.FullName + "." + invocation.Method.Name + taskDescriptor + " returning with: " + ToJson(result));
                            });
                        }
                        else
                        {
                            _logger.Info("Returning with: " + ToJson(returnValue));
                        }
                    }
                }
                else if (_logger.IsDebugEnabled)
                {
                    _logger.Debug(string.Format("Exiting: {0}.{1}, return value: {2}", invocation.TargetType.FullName, invocation.Method.Name, invocation.ReturnValue.ToString()));
                }

            }
            catch (Exception ex)
            {
                if (_logger.IsErrorEnabled)
                {
                    _logger.Error(CreateInvocationLogString("ERROR", invocation), ex);
                }
                throw;
            }
        }
        #endregion

        #region Private Methods
        private string CreateInvocationLogString(string operation, IInvocation invocation)
        {
            var sb = new StringBuilder(100);
            sb.AppendFormat("{0}: {1}.{2}( ", operation, invocation.TargetType.FullName, invocation.Method.Name);
            foreach (var argument in invocation.Arguments)
            {
                String argumentDescription = argument == null ? "null" : DumpObject(argument);
                sb.Append(argumentDescription).Append(",");
            }
            if (invocation.Arguments.Any())
            {
                sb.Length -= 1;
            }
            sb.Append(")");
            return sb.ToString();
        }

        private static string DumpObject(object argument)
        {
            Type objtype = argument.GetType();
            if (objtype == typeof(String) || objtype.IsPrimitive || !objtype.IsClass)
            {
                return argument.ToString();
            }

            return ToJson(argument);
        }

        private static string ToJson(object value)
        {
            //TODO:Revisit here as we have implemented try catch here.
            //Try
            //    Return JsonConvert.SerializeObject(value, Formatting.None, New JsonSerializerSettings() With {.ReferenceLoopHandling = ReferenceLoopHandling.Ignore})
            //Catch ex As Exception
            //    Return value.ToString()
            //End Try
            return value.ToString();

        }
        #endregion
    }
}
