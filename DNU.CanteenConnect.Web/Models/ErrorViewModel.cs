// DNU.CanteenConnect.Web/Models/ErrorViewModel.cs
using System;

namespace DNU.CanteenConnect.Web.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}