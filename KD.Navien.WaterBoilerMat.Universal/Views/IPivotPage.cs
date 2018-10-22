using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.Universal.Views
{
    public interface IPivotPage
    {
        Task OnPivotSelectedAsync();

        Task OnPivotUnselectedAsync();
    }
}
