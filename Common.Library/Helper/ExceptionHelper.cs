using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.Threading.Tasks;
using System.Configuration;

namespace Common.Library.Helper
{
    public static class ExceptionHelper
    {
        private static readonly string DEFAULT_EXCEPTION_POLICY = "DefaultExceptionPolicy";

        // Lazy init fields to avoid type initializer exceptions on platforms where
        // Enterprise Library or configuration file is missing.
        private static FileConfigurationSource? exceptionsSource = null;
        private static ExceptionPolicyFactory? exceptionFactory = null;
        private static bool initialized = false;

        private static void EnsureInitialized()
        {
            if (initialized) return;

            try
            {
                exceptionsSource = new FileConfigurationSource("Common.Library.dll.config");
                exceptionFactory = new ExceptionPolicyFactory(exceptionsSource);
            }
            catch (Exception ex)
            {
                // Log to console for debugging startup issues; do not crash application.
                try { Console.WriteLine("ExceptionHelper initialization failed: " + ex.Message); } catch { }
                exceptionsSource = null;
                exceptionFactory = null;
            }
            finally
            {
                initialized = true;
            }
        }

        private static ExceptionPolicyImpl? CreateExceptionHandlingPolicy(string policyName)
        {
            EnsureInitialized();

            if (exceptionFactory == null)
                return null;

            AppSettingsSection? appSettingsSection = exceptionsSource?.GetSection("appSettings") as AppSettingsSection;
            if (appSettingsSection != null && appSettingsSection.Settings.Count > 0)
            {
                KeyValueConfigurationElement? defaultPolicyName = appSettingsSection.Settings["DefaultExceptionPolicyName"];
                if (defaultPolicyName == null)
                {
                    policyName = ExceptionHelper.DEFAULT_EXCEPTION_POLICY;
                }
                else
                {
                    policyName = defaultPolicyName.Value;
                }
            }
            else
            {
                policyName = ExceptionHelper.DEFAULT_EXCEPTION_POLICY;
            }

            if (string.IsNullOrEmpty(policyName))
                policyName = ExceptionHelper.DEFAULT_EXCEPTION_POLICY;

            try
            {
                ExceptionPolicyImpl result = exceptionFactory.Create(policyName);
                return result;
            }
            catch (Exception ex)
            {
                try { Console.WriteLine("ExceptionHelper.CreateExceptionHandlingPolicy failed: " + ex.Message); } catch { }
                return null;
            }
        }

        public static void HandleException(Exception ex)
        {
            HandleException(ex, string.Empty);
        }

        public static void HandleException(string message, Exception innerException)
        {
            Exception newException = new Exception(message, innerException);
            HandleException(newException);
        }

        public static void HandleException(Exception ex, string policyName)
        {
            var exceptionPolicy = CreateExceptionHandlingPolicy(policyName);
            if (exceptionPolicy == null)
            {
                // Fallback behaviour: log to console for debugging
                try 
                { 
                    Console.WriteLine($"ExceptionHelper: no exception policy available.");
                    Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                    Console.WriteLine($"Exception Message: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                } 
                catch { }
                // Don't rethrow - just log for debugging
                return;
            }

            if (exceptionPolicy.HandleException(ex))
                throw ex;
        }

        public static void HandleException(string message, Exception innerException, string policyName)
        {
            Exception newException = new Exception(message, innerException);
            HandleException(newException, policyName);
        }
    }
}
