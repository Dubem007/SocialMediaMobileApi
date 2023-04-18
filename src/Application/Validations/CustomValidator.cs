using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Validations
{
    public static class CustomValidator
    {
        public static IRuleBuilderOptions<T, IFormFile> NotMoreThan40MB<T>(this IRuleBuilder<T, IFormFile> ruleBuilder)
        {
            return ruleBuilder.Must(x => x.Length <= 40000000);
        }

        public static IRuleBuilderOptions<T, IFormFile> CheckFileFormat<T>(this IRuleBuilder<T, IFormFile> ruleBuilder, string fileFormat)
        {
            return ruleBuilder.Must(x => fileFormat.Contains(Path.GetExtension(x.FileName).ToLower()));
        }
    }
}
