   M S B A M L   `   `   `  ���� G  CBimEraPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullA%clr-namespace:BimEraPlugin.ViewModelsBimEraPlugin.ViewModels  7 clr-namespace:BimEraPlugin.ViewsBimEraPlugin.Views  +    %BimEraPlugin.Views.RoomsToViewsWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  .vm%clr-namespace:BimEraPlugin.ViewModels  ,local clr-namespace:BimEraPlugin.Views    � Title$  RoomsToViewsWindow��$	��450��$	��550�� � WindowStartupLocation$ CenterScreen=� � 	Resources +   %BimEraPlugin.Views.RoomsToViewsWindow �� 
TargetType �� 	Resources �� MergedDictionaries �� Source �� Property �� Value   PrimaryHueMidBrush  MaterialDesignPaper	 �� 
TargetType(  "MaterialDesignThemes.Wpf.ColorZone
  Name   Mode+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth � ColumnDefinitions $  MaterialDesignHeadline5TextBlock �� TextAlignment �� Orientation'  !System.Windows.Shell.WindowChrome   IsHitTestVisibleInChrome'  !MaterialDesignThemes.Wpf.PackIcon   Kind)  #MaterialDesignThemes.Wpf.DialogHost   CloseOnClickAway   
Identifier � RowDefinitions �� DisplayMemberPath �� SelectedItem d� 	IsChecked)  #MaterialDesignThemes.Wpf.HintAssist   Hint �� UpdateSourceTrigger � IsSharedSizeScope �� DataType0   *BimEraPlugin.ViewModels.RoomsDataViewModel ��Name �� Padding z� SharedSizeGroup �� Triggers  �� Value! �� Binding" �� 
TargetName  MaterialDesignSelection��%|
  '        �� "   ��  �� $e _pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml?��� $y spack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.LightBlue.xaml?��� $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?��� $x rpack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.LightBlue.xaml?�.���  �@�WindowStyle$
 None=���  � ��$ 2 0 2 2q���  � ��# �   ��  � ��# �  ��  � ��$ 	Tw Cen MT)���  � �� �� "	 �.R�� 
�������.+�]� ��. .+�  $
 	colorZone��$	��Top=�$��35��$ 
PrimaryMid=�$ Depth2=�.���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�(� $
��Left=�$��0��$��8��$��30��$��100,0q�.���� #���  $��Center=�$��Center=�$ Center=�$��Rooms To Views��$
��Bold"�$��5q�
��������� $��7��$ 
Horizontal=�$��Right=�.+��� -    . $��1q�
�������.��  $ WindowMinimize=�
��������� -    . $��1q�
�������.��  $ WindowRestore=�
��������� -    . $��1q�
�������.��  $ WindowClose=�
��������� #��z��#��z��._�� .+�   .  $ MainContent��.���  �� $��* � �� $��* ��� $��60 �.+��� $ Vertical=�.+��� $ 
Horizontal=�.+��� $��Model��$��17(�$��10q�$��Center=��� $��10q�$ Title��$	��200��$	��Top=����� *DocumentList+ �� *SelectedDocument+�� $��17(�$��10q�$��Center=�	Include�� Spaces��  �� *IsSpaceChecked+�� $��Offset��$��17(�$��10q�$��Center=��� -   $��10q�$��80��$ in mm��$��Center=����� *Offset+�� $ 
Horizontal=�.+��� $��17(�$��10q�$
��Name��$ Center=�$��Center=��� $��17(�$��10q�$	��225��$ Filter Rule������ *FilterRuleList+ �� *SelectedFilterRule+�� $��17(�$��10q�$	��200������ *
FilterText+$ PropertyChanged=��� $��10q�$��0��$	��240��$
��Auto=�$
��Auto=�.����  . $��	12 0 12 0q����� *	ItemsView+���� "  (��       *, +).R��� $ Border��$ 8q�._��  �� $ Checkerz���� .+��� $��Center=� �� *	IsChecked+�� $��8 0 0 0q�$��1��.+��� $
��Bold"����� *Name+ �� $
  True��! �� *	IsChecked+.a��� $" Border��# �   � ���� $ 
Horizontal=�$��1��$��2��$��Center=�.+��� $��0,0,20,0q�
�������$��Create Views��$	��300��$��Center=����� *CreateViewCommand+#   WindowChrome#   $   CaptionHeight$$ 35Q�%   CornerRadius$% 0��&   ResizeBorderThickness$& 5q�'   GlassFrameThickness$' 0q�