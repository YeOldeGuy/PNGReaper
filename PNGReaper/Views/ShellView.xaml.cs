using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using PNGReaper.Helpers;
using PNGReaper.Services.Abstract;
using PNGReaper.ViewModels;
using Prism.Events;

namespace PNGReaper.Views;

public partial class ShellView
{
    private readonly IPersistService        _persistService;
    private readonly IWindowPositionService _positionService;
    private          bool                   _activated;
    private          ShellViewModel?        _vm;

    public ShellView(IPersistService persistService,
        IWindowPositionService positionService)
    {
        _persistService  = persistService;
        _positionService = positionService;
        InitializeComponent();
        SetRoundedCorners();
    }

    protected override void OnActivated(EventArgs e)
    {
        if (_activated) return;

        SizeChanged += OnSizeChanged;
        
        _vm        = (ShellViewModel)DataContext;
        _activated = true;

        var pos = _persistService.StartPosition;

        if (pos.length == 0)
            _persistService.StartPosition = _positionService.GetPosition(this);
        else
            _positionService.SetPosition(this, pos);

        base.OnActivated(e);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var pos = _positionService.GetPosition(this);
        _persistService.StartPosition = pos;
        _persistService.Save();
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        // Don't do this if the window hasn't been activated yet. If
        // you do, you'll just write the default position to the persist

        if (!_activated)
            return;

        var pos = _positionService.GetPosition(this);
        _persistService.StartPosition = pos;
        _persistService.Save();
    }

    private static bool IsPngInDropList(DataObject data)
    {
        if (data.ContainsFileDropList())
        {
            var list = data.GetFileDropList();
            var first = list[0];
            if (Path.GetExtension(first)!.Equals(".png", StringComparison.InvariantCultureIgnoreCase))
                return true;
        }

        return false;
    }

    private void PNGPreviewDrop(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        if (e.Data is DataObject data)
            if (IsPngInDropList(data))
                e.Effects = DragDropEffects.All;
    }

    private void PNGPreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        if (e.Data is DataObject data)
            if (IsPngInDropList(data))
                e.Effects = DragDropEffects.All;
    }

    private void PNGDragDrop(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        if (e.Data is DataObject data)
            if (IsPngInDropList(data))
            {
                e.Effects = DragDropEffects.All;
                var list = data.GetFileDropList();
                var first = list[0];
                if (_vm is null || first is null) return;

                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption   = BitmapCacheOption.OnLoad;
                bi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bi.UriSource     = new Uri(first);
                bi.EndInit();

                pngImage.Source = bi;
                _vm.ImageFile = first;
            }

        e.Handled = true;
    }
}