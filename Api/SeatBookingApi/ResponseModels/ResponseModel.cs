using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeatBookingApi.ResponseModels
{
    public class ResponseModel
    {
        public string Message { get; set; }
        public bool Status { get; set; } = false;
        public IEnumerable<string> Errors { get; set; }
        public object Data { get; set; }

        public static ResponseModel SuccessResponse(string message)
        {
            return new ResponseModel
            {
                Data = null,
                Errors = null,
                Message = message,
                Status = true
            };
        }
        public static ResponseModel SuccessResponse(object data, string message = "")
        {
            return new ResponseModel
            {
                Data = data,
                Errors = null,
                Message = message,
                Status = true
            };
        }

        public static ResponseModel ErrorResponse(string error)
        {
            List<string> errors = new List<string>
            {
                error
            };
            return new ResponseModel
            {
                Data = null,
                Errors = errors,
                Message = "",
                Status = false
            };
        }
        public static ResponseModel ErrorResponse(List<string> errors)
        {
            return new ResponseModel
            {
                Data = null,
                Errors = errors,
                Message = "",
                Status = false
            };
        }
        public static ResponseModel ErrorResponse(List<string> errors, object data)
        {
            return new ResponseModel
            {
                Data = data,
                Errors = errors,
                Message = "",
                Status = false
            };
        }
        public static ResponseModel ErrorResponse(string errorMessage, object data)
        {
            List<string> errors = new List<string>
            {
                errorMessage
            };
            return new ResponseModel
            {
                Data = data,
                Errors = errors,
                Message = "",
                Status = false
            };
        }
    }
    public enum EStatusCode
    {
        Success = 0,
        Failed = 1
    }
}
