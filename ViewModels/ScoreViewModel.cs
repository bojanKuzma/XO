using System.Collections.ObjectModel;
using XO.Models;

namespace XO.ViewModels;

public class ScoresViewModel : BaseViewModel
{
    public ObservableCollection<ScoreModel> Scores { get; } = new();
}