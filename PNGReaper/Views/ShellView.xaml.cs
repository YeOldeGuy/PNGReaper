using System;
using System.IO;
using System.Windows;
using PNGReaper.ViewModels;

namespace PNGReaper.Views;

public partial class ShellView
{
    private ShellViewModel? _vm;

    public ShellView()
    {
        InitializeComponent();
        SetRoundedCorners();
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        _vm = (ShellViewModel)DataContext;
    }

    private static bool IsPngInDropList(DataObject data)
    {
        if (data.ContainsFileDropList())
        {
            var list = data.GetFileDropList();
            var first = list[0];
            if (Path.HasExtension(".png")) return true;
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

    // private void PNGDragEnter(object sender, DragEventArgs e)
    // {
    // }

    private void PNGDragDrop(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.None;
        if (e.Data is DataObject data)
            if (IsPngInDropList(data))
            {
                e.Effects = DragDropEffects.All;
                var list = data.GetFileDropList();
                var first = list[0];
                if (_vm is null) return;
                _vm.ImageFile = first;
            }

        e.Handled = true;
    }
}