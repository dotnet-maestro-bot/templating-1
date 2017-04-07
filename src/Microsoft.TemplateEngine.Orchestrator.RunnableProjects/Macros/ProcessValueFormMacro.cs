﻿using System;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.TemplateEngine.Core.Contracts;
using Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros.Config;
using Microsoft.TemplateEngine.Utils;

namespace Microsoft.TemplateEngine.Orchestrator.RunnableProjects.Macros
{
    public class ProcessValueFormMacro : IMacro, IDeferredMacro
    {
        public string Type => "processValueForm";

        public Guid Id => new Guid("642E0443-F82B-4A4B-A797-CC1EB42221AE");

        public void EvaluateConfig(IEngineEnvironmentSettings environmentSettings, IVariableCollection vars, IMacroConfig config, IParameterSet parameters, ParameterSetter setter)
        {
            ProcessValueFormMacroConfig realConfig = config as ProcessValueFormMacroConfig;

            if (realConfig == null)
            {
                throw new InvalidCastException("Couldn't cast the rawConfig as a ProcessValueFormMacroConfig");
            }

            string value;
            if (!vars.TryGetValue(realConfig.SourceVariable, out object working))
            {
                if (parameters.TryGetRuntimeValue(environmentSettings, realConfig.SourceVariable, out object resolvedValue, true))
                {
                    value = resolvedValue.ToString();
                }
                else
                {
                    value = string.Empty;
                }
            }
            else
            {
                value = working?.ToString() ?? "";
            }

            value = realConfig.Forms[realConfig.Form].Process(realConfig.Forms, value);

            Parameter p = new Parameter
            {
                IsVariable = true,
                Name = config.VariableName
            };

            vars[config.VariableName] = value;
            setter(p, value);
        }

        public void EvaluateDeferredConfig(IEngineEnvironmentSettings environmentSettings, IVariableCollection vars, IMacroConfig rawConfig, IParameterSet parameters, ParameterSetter setter)
        {
            EvaluateConfig(environmentSettings, vars, rawConfig, parameters, setter);
        }
    }
}
