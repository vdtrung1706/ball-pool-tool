using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace pool_tool.Class {
    public class TableConfig : INotifyPropertyChanged {
        private double _height;
        public double height {
            get {
                return _height;
            }
            set {
                _height = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("height"));
            }
        }

        private double _width;
        public double width {
            get {
                return _width;
            }
            set {
                _width = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("width"));
            }
        }
        public double top { get; set; }
        public double left { get; set; }
        public double right { get; set; }
        public double bottom { get; set; }

        private double _ballSize;
        public double ballSize {
            get {
                return _ballSize;
            }
            set {
                _ballSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ballSize"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
