   M S B A M L   `   `   `  ���� A  =BimEra, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:BimEra.ViewsBimEra.Views  5clr-namespace:BimEra.ViewModelsBimEra.ViewModels  /    )BimEra.Views.CalculateNetWallLengthWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  &localclr-namespace:BimEra.Views  (vmclr-namespace:BimEra.ViewModels    � Title$"  CalculateNetWallLengthWindow��$	��600��$	��550�� � WindowStartupLocation$ CenterScreen=� � 	Resources /   )BimEra.Views.CalculateNetWallLengthWindow �� 
TargetType �� 	Resources �� MergedDictionaries �� Source �� Property �� Value   PrimaryHueMidBrush  MaterialDesignPaper	 �� 
TargetType(  "MaterialDesignThemes.Wpf.ColorZone
  Name   Mode+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth � ColumnDefinitions $  MaterialDesignHeadline5TextBlock �� Orientation'  !System.Windows.Shell.WindowChrome   IsHitTestVisibleInChrome'  !MaterialDesignThemes.Wpf.PackIcon   Kind)  #MaterialDesignThemes.Wpf.DialogHost   CloseOnClickAway   
Identifier � RowDefinitions $  MaterialDesignHeadline6TextBlock � IsSharedSizeScope �� DataType/   )BimEra.ViewModels.WallTypePickerViewModel ��Name �� Padding z� SharedSizeGroup d� 	IsChecked �� Triggers �� Value �� Binding �� 
TargetName  MaterialDesignSelection��%  '        �� "   ��  �� $e _pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml?��� $y spack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.Lightblue.xaml?��� $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?��� $s mpack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.Lime.xaml?�.���  �@�WindowStyle$
 None=���  � ��$ 2 0 2 2q���  � ��# �   ��  � ��# �  ��  � �� �� "	 �.R�� 
�������.+�]� ��. .+�  $
 	colorZone��$	��Top=�$��35��$ 
PrimaryMid=�$ Depth2=�.���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�(� $��0��$��8��$��30��$��100,0q�.���� #���  $��Center=�$��Center=�$��Actual Sweep Length��$��5q�
��������� $��7��$ 
Horizontal=�$��Right=�.+��� -    . $��1q�
�������.��  $ WindowMinimize=�
��������� -    . $��1q�
�������.��  $ WindowRestore=�
��������� -    . $��1q�
�������.��  $ WindowClose=�
��������� #��z��#��z��._�� .+�   .  $ MainContent��.���  �� $��* � �� $��* ��� $��60 �.+��� $ Vertical=�.+��� $	��150��$��10q�$��Center=�$��Pick Wall Types:��#���  $��Center=��� $��10q�$��0��$	��400��$
��Auto=�$
��Auto=�.����  . $��	12 0 12 0q����� *	WallTypes+���� "  (��       *, +).R��� $ Border��$ 8q�._��  �� $ Checkerz���� .+��� $��Center=� �� *	IsChecked+�� $��8 0 0 0q�$��1��.+��� $
��Bold"����� *Name+ �� $
 True�� �� *	IsChecked+.a��� $ Border��# �   � ���� $ 
Horizontal=�$��1��$��2��$��Center=�.+��� $��0,0,20,0q�
�������$��Calculate Sweep Length��$	��300��$��Center=����� *CalculateWallLengthCommand+   WindowChrome      CaptionHeight$ 35Q�    CornerRadius$  0��!   ResizeBorderThickness$! 5q�"   GlassFrameThickness$" 0q�