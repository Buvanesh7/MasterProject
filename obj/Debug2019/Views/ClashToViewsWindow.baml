   M S B A M L   `   `   `  ���� G  CBimEraPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:BimEraPlugin.ViewsBimEraPlugin.Views  A%clr-namespace:BimEraPlugin.ViewModelsBimEraPlugin.ViewModels  A%clr-namespace:BimEraPlugin.ConvertersBimEraPlugin.Converters  +    %BimEraPlugin.Views.ClashToViewsWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     2local clr-namespace:BimEraPlugin.Views     4vm%clr-namespace:BimEraPlugin.ViewModels     b ^MaterialDesignThemes.Wpf, Version=3.2.0.1979, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  6
converters%clr-namespace:BimEraPlugin.Converters    � Title$  ClashToViewsWindow��$	��485��$	��700�� � WindowStartupLocation$ CenterScreen=� � 	Resources +   %BimEraPlugin.Views.ClashToViewsWindow �� 
TargetType �� 	Resources �� MergedDictionaries �� Source3   -BimEraPlugin.Converters.ColorToBrushConverter   Key6   0BimEraPlugin.Converters.StringToIntegerConverter �� Property �� Value  PrimaryHueMidBrush  MaterialDesignPaper	 �� 
TargetType(  "MaterialDesignThemes.Wpf.ColorZone
  Name   Mode+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth � ColumnDefinitions $  MaterialDesignHeadline5TextBlock �� TextAlignment �� Orientation'  !System.Windows.Shell.WindowChrome   IsHitTestVisibleInChrome'  !MaterialDesignThemes.Wpf.PackIcon   Kind)  #MaterialDesignThemes.Wpf.DialogHost   CloseOnClickAway   
Identifier � RowDefinitions �� DisplayMemberPath)	  #MaterialDesignThemes.Wpf.HintAssist 	  Hint �� SelectedItem#
  MaterialDesignThemes.Wpf.Card.  (MaterialDesignThemes.Wpf.TextFieldAssist   HasClearButton �� UpdateSourceTrigger ��Name � IsSharedSizeScope �� DataType5   /BimEraPlugin.ViewModels.CategoryPickerViewModel ��Name �� Padding z� SharedSizeGroup  d� 	IsChecked! �� Triggers" �� Value# �� Binding$ �� 
TargetName  MaterialDesignSelection��%  '        �� "   ��  �� $e _pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml?��� $y spack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.LightBlue.xaml?��� $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?��� $x rpack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.LightBlue.xaml?�  ColorToBrushConverter    StringToIntegerConverter  .���  �@�WindowStyle$
 None=���  � ��$ 2 0 2 2q���  � ��# �  ��  � ��# �  ��  � ��$ 	Tw Cen MT)���  � �� �� "	 �.R�� 
�������.+�]� ��. .+�  $
 	colorZone��$	��Top=�$��35��$ 
PrimaryMid=�$ Depth2=�.���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�(� $
��Left=�$��0��$��8��$��30��$��	250,0,0,0q�.���� #���  $��Center=�$��40(�$��Center=�$ Center=�$��Clash To View��$
��Bold"�$��5q�
��������� $��7��$ 
Horizontal=�$��Right=�.+��� -    . $��1q�
�������.��  $ WindowMinimize=�
��������� -    . $��1q�
�������.��  $ WindowRestore=�
��������� -    . $��1q�
�������.��  $ WindowClose=�
��������� #��z��#��z��._�� .+�   .  $ MainContent��.���  �� $��* ��� $��* � �� $
��Auto ��� $
��Auto ��� $��60 �.+��� $��10q�$ Title��$	��225��$	��Top=�$ Model A������ *DocumentListA+ �� *SelectedDocumentA+�� $��10q�$��1��$ Title��$	��225��$	��Top=�$ Model B������ *DocumentListB+ �� *SelectedDocumentB+
  $	��300��$��10q�$��1��.���� $ Vertical=�.+��� $��5q�$��30��$ Filter�� . ���� *FilterTextA+$ PropertyChanged=��� $��10q�$��0��$	��250��$
��Auto=�$
��Auto=�.���� $ AList�� . $��	12 0 12 0q����� *
ItemsView1+���� "  (��       *, +).R��� $ Border��$ 8q�._��  �� $ Checkerz���� .+��� $��Center=�  �� *	IsChecked+�� $��8 0 0 0q�$��1��.+��� $
��Bold"����� *Name+! �� $
" True��# �� *	IsChecked+.a��� $$ Border��# �   � ��
  $	��300��$��10q�$��1��$��1��.���� $ Vertical=�.+��� $��5q�$��30��$ Filter�� . ���� *FilterTextB+$ PropertyChanged=��� $��10q�$��0��$	��250��$
��Auto=�$
��Auto=�.���� $ BList�� . $��	12 0 12 0q����� *
ItemsView2+���� "  (��       *, +).R��� $ Border��$ 8q�._��  �� $ Checkerz���� .+��� $��Center=�  �� *	IsChecked+�� $��8 0 0 0q�$��1��.+��� $
��Bold"����� *Name+! �� $
" True��# �� *	IsChecked+.a��� $$ Border��# �   � ���� $ 
Horizontal=�$��2��$��2��$��Center=�.+��� $��0,0,20,0q�
�������$��Create Clash Views��$	��300��$��Center=����� *CreateClashViewsCommand+%   WindowChrome%   &   CaptionHeight$& 35Q�'   CornerRadius$' 0��(   ResizeBorderThickness$( 5q�)   GlassFrameThickness$) 0q�