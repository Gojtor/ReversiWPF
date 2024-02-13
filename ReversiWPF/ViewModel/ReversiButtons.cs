using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiWPF.ViewModel
{
    public class ReversiButtons : ViewModelBase
    {
        private String text = String.Empty;
        private bool isValid;
        public String Text
        {
            get { return text; }
            set {
                if (text != value)
                {
                    text = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsValid
        {
            get { return isValid; }
            set {
                if (isValid != value)
                {
                    isValid = value;
                    OnPropertyChanged();
                }
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public Tuple<int,int> XY
        {
            get { return new(X,Y); }
        }
        public DelegateCommand? PutPieceCommand { get; set; }
    }
}
