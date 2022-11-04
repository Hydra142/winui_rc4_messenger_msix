using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using SafeMessenge.Models;
using SafeMessenge.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeMessenge.ViewModels;

public class ShellPageViewModel : ObservableRecipient
{
    public NavigationService NavigationService { get; set; }
    public AppDataService AppDataService { get; set; }
    private User? _CurrentUser;
    public User? CurrentUser {
        get => _CurrentUser;
        set 
        { 
            SetProperty(ref _CurrentUser, value); 
        } 
    }


    public ShellPageViewModel(NavigationService navigationService, AppDataService appDataService)
    {
        NavigationService = navigationService;
        AppDataService = appDataService;
    }
}
