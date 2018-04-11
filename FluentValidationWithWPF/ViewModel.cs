using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidationWithWPF
{
    public class ViewModel : IViewModel
    {
        public ViewModel(IView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException("view");
            }

            View = view;
            View.ViewModel = this;
        }

        private IView _view;
        public IView View
        {
            get { return _view; }
            set
            {
                if (_view != value)
                {
                    _view = value;
                    OnPropertyChanged(() => this.View);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            var propertyChanged = PropertyChanged;

            if (propertyChanged == null)
            {
                return;
            }

            propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpresion)
        {
            var property = propertyExpresion.Body as MemberExpression;
            if (property == null || !(property.Member is PropertyInfo) ||
                !IsPropertyOfThis(property))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    "Expression must be of the form 'this.PropertyName'. Invalid expression '{0}'.",
                    propertyExpresion), "propertyExpression");
            }

            this.OnPropertyChanged(property.Member.Name);
        }

        private bool IsPropertyOfThis(MemberExpression property)
        {
            var constant = RemoveCast(property.Expression) as ConstantExpression;
            return constant != null && constant.Value == this;
        }

        private Expression RemoveCast(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Convert ||
                expression.NodeType == ExpressionType.ConvertChecked)
                return ((UnaryExpression)expression).Operand;

            return expression;
        }

        public virtual ValidationResult SelfValidate()
        {
            return new ValidationResult();
        }

        private string GetValidationError(string property = null)
        {
            var validationResult = SelfValidate();

            if (validationResult == null || validationResult.IsValid)
            {
                IsValid = true;
                return string.Empty;
            }

            IsValid = false;

            if (property == null)
            {
                return string.Join(Environment.NewLine, validationResult.Errors.Select(e => e.ErrorMessage));
            }
            else
            {
                var results = validationResult.Errors.FirstOrDefault(e => e.PropertyName == property);

                return results != null ? results.ErrorMessage : string.Empty;
            }
        }

        public string Error
        {
            get
            {
                return GetValidationError();
            }
        }

        public string this[string property]
        {
            get
            {
                return GetValidationError(property);
            }
        }        

        public bool IsValid { get; private set; }

        protected bool Validate()
        {
            IsValid = SelfValidate().IsValid;

            return IsValid;
        }
    }
}
