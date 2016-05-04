using System;
using System.ComponentModel;

/// <summary>
/// This class provides example how to use IDataErrorInfo interface to validate property in UI
/// </summary>
namespace Sample
{
    public class IDataErrorInfoViewModel: IDataErrorInfo
    {
        private string _propertyToValidate;

        public string PropertyToValidate
        {
            get
            {
                return _propertyToValidate;
            }
            set
            {
                _propertyToValidate = value;
                //RaisePropertyChanged("PropertyToValidate");
            }
        }

        #region IDataErrorInfo Members

        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return this.GetValidationError(propertyName); }
        }

        /// <summary>
        /// Returns true if this object has no validation errors.
        /// </summary>
        public bool IsValid()
        {
            foreach (string property in ValidatedProperties)
                if (GetValidationError(property) != null)
                {
                    return false;
                }

            return true;
        }

        static readonly string[] ValidatedProperties =
        {
            "PropertyToValidate",
        };

        string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string error = null;

            switch (propertyName)
            {
                case "PropertyToValidate":
                    error = ValidateNumberEmptyOrZero(PropertyToValidate);
                    break;
                default:
                    break;
            }

            return error;
        }

        static bool IsStringMissing(string value)
        {
            return
                String.IsNullOrEmpty(value) ||
                value.Trim() == String.Empty;
        }

        string ValidateNumberEmptyOrZero(string value)
        {
            string errorMessage = null;
            float valueFloat;

            if (IsStringMissing(value))
            {
                errorMessage =  "Value can't be empty";
            }
            else if (!float.TryParse(value, out valueFloat))
            {
                errorMessage =  "Invalid value";
            }
            else if (valueFloat == 0)
            {
                errorMessage = "Value can't be zero";
            }
            else if (valueFloat < 0.0)
            {
                errorMessage = "Value can't be negative";
            }

            return errorMessage;
        }

        #endregion 
    }
}
