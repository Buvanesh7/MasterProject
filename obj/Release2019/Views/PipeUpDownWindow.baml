   M S B A M L   `   `   `  ���� G  CBimEraPlugin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:BimEraPlugin.ViewsBimEraPlugin.Views  )    #BimEraPlugin.Views.PipeUpDownWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     _ [MaterialDesignThemes.Wpf, Version=4.3.0.3, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  ,local clr-namespace:BimEraPlugin.Views    � Title$   CreateSmokeDetectorsWindow��$	��275��$	��375�� � WindowStartupLocation$ CenterScreen=� � 	Resources )   #BimEraPlugin.Views.PipeUpDownWindow �� 
TargetType �� 	Resources �� MergedDictionaries �� Source �� Property �� Value   PrimaryHueMidBrush  MaterialDesignPaper	 �� 
TargetType(  "MaterialDesignThemes.Wpf.ColorZone
  Name   Mode+  %MaterialDesignThemes.Wpf.ShadowAssist   ShadowDepth � ColumnDefinitions $  MaterialDesignHeadline5TextBlock �� Orientation'  !System.Windows.Shell.WindowChrome   IsHitTestVisibleInChrome'  !MaterialDesignThemes.Wpf.PackIcon   Kind)  #MaterialDesignThemes.Wpf.DialogHost   CloseOnClickAway   
Identifier � RowDefinitions & "MaterialDesignFloatingActionButton)  #MaterialDesignThemes.Wpf.HintAssist   Hint �� UpdateSourceTrigger��%�  '        �� "   ��  �� $e _pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Light.xaml?��� $y spack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor.LightBlue.xaml?��� $h bpack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Defaults.xaml?��� $x rpack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Accent/MaterialDesignColor.LightBlue.xaml?�.���  �@�WindowStyle$
 None=���  � ��$ 2 0 2 2q���  � ��# �   ��  � ��# �  ��  � ��$ 	Tw Cen MT)���  � �� �� "	 �.R�� 
�������.+�]� ��. .+�  $
 	colorZone��$	��Top=�$��35��$ 
PrimaryMid=�$ Depth2=�.���  �� $
��Auto ��� $
��Auto ��� $
��Auto ��� �� $
��Auto ��� $��* ��� $
��Auto ��� $
��Auto �.+�(� $��0��$��6��$��30��$
��50,0q�.���� #���  $��Center=�$��Center=�$��MEP Re-Router��$
��Bold"�$��5q�
��������� $��7��$ 
Horizontal=�$��Right=�.+��� -    . $��1q�
�������.��  $ WindowMinimize=�
��������� -    . $��1q�
�������.��  $ WindowRestore=�
��������� -    . $��1q�
�������.��  $ WindowClose=�
��������� #��z��#��z��._�� .+�   .  $ MainContent��.���  �� $��* ��� $	��200 ��� $��* � �� $��* ��� $��* ��� $��* �.+��� $��Center=�$��1��#���  ���� *	UpCommand+.��  $ 	ChevronUp=�$��50��$��50��
��������� $��Center=�$��1��#���  ���� *LeftCommand+.��  $ ChevronLeft=�$��50��$��50��
��������� $��Center=�$��2��$��1��#���  ���� *RightCommand+.��  $ ChevronRight=�$��50��$��50��
��������� $��Center=�$��1��$��2��#���  ���� *DownCommand+.��  $ ChevronDown=�$��50��$��50��
��������� $ Vertical=�$��1��$��1��$��Center=�$��Center=�.+��� $��10q�$��Center=�$��20(�$	��150��$��30��$ Offset in mm������ *Offset+$ PropertyChanged=��� $��10q�$��Center=�$��20(�$	��150��$��30��$ Angle in deg������ *Angle+$ PropertyChanged=�   WindowChrome      CaptionHeight$ 35Q�   CornerRadius$ 0��   ResizeBorderThickness$ 5q�   GlassFrameThickness$ 0q�