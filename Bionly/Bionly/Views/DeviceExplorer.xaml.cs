using Bionly.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Bionly.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeviceExplorer : TabbedPage
    {
        public DeviceExplorer()
        {
            InitializeComponent();
        }

        private new void Appearing(object sender, EventArgs e)
        {
            //((DeviceExplorerViewModel)BindingContext).ConnectFTPS.Execute(null);
        }
    }
}