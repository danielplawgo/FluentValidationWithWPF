using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidationWithWPF
{
    public interface IViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        IView View { get; set; }

        bool IsValid { get; }
    }
}
