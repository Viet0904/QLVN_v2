using Newtonsoft.Json;
using System.Collections.Generic;

namespace Common.Model.Common
{
    public class ResModel<T>
    {
        public ResModel()
        {
            IsSuccess = true;
            Errors = new List<ErrorModel>();
        }

        private bool isSuccess;

        [JsonProperty("isSuccess")]
        public bool IsSuccess
        {
            get
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    return false;
                }
                return isSuccess;
            }

            set
            {
                isSuccess = value;
            }
        }

        [JsonProperty("data")]
        public T Data { get; set; } = default!;

        [JsonProperty("errorMessage")]
        public string? ErrorMessage { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("errorDetails")]
        public List<ErrorModel> Errors { get; set; } = null!;
    }
}
