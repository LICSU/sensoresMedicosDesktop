   M S B A M L   `   `   `  ����l  hSystem.Windows.Controls.DataVisualization.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=null�rclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit)System.Windows.Controls.DataVisualization  5      �{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit2System.Windows.Controls.DataVisualization.Charting  5       
AppSensCom   AppSensCom.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      �DVrclr-namespace:System.Windows.Controls.DataVisualization;assembly=System.Windows.Controls.DataVisualization.Toolkit     5      �DVC{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit     5        � Title$  Comunication��5      $��586.741��6   $��840.69��6)    � 
ResizeMode$ NoResize=�68    � WindowStartupLocation$ CenterScreen=�6N   ���	#FFF0EAEA��5      �� 5       �� EndPoint$ 0.5,1&�6    � MappingMode$ RelativeToBoundingBox=�6)    �� 
StartPoint$ 0.5,0&�6M   .��5	      �  � Color$ 	#FFE8EEF4��6    � Offset$ 1Q�6$   � 5
      $ 	#FF487FB9��6   $ 0.717Q�6$   � 5      $ 	#FFDDE6F0��6   $ 1Q�6$   5      .��5      � .+�5      �� $��Comunicación Sensores��6    �� TextAlignment$ Center=�6+   $��16(�6B   $
��Bold"�6P   $��243,20,215,0q�6b   �� 5      -   5        	 ��Name$	 LblPeso��5   L   $��Paciente��6   $ Center=�6   $��68,332,667,30q�65   6L   �� 5      -   5        $	 LblPulsiometro��5   L   $��Paciente��6   $ Center=�6   $��679,332,26,30q�65   6L   �� 5      -   5        $	 LblGlucometro��5   M   $��Paciente��6   $ Center=�6   $��521,332,184,30q�65   6M   �� 5      -   5        $	 
LblElectro��5   M   $��Paciente��6   $ Center=�6   $��369,332,336,30q�65   6M   �� 5      -   5        $	 	LblMedida��5   M   $��Paciente��6   $ Center=�6   $��207,332,498,30q�65   6M   � 5      $��Center=�6	   $	��Top=�6&   $��80��6>   $	��331��6J   $��247,60,257,0q�6V   
 � ColumnDefinitions
 5      �� 5      $��0* �6   �� 5      �� 5       � RowDefinitions 5      �� 5      �� 5      $��0* �6   �� 5      $	��20* �6   �� 5      $	��20* �6   �� 5       $	��20* �6   .+�5"      �� $��Puerto:��6   $ Right=�6    �� Padding$ 0,0,10,0q�64   $��0��6G   $��2��6T   $��3��6h   �� 5#      $��3��6   $ ��Velocidad de Transmisión:��6   $ Center=�6=   $ 0,0,10,0q�6T   $��Right=�6g   $	��150��6�   $��2��6�   �� 5$      -   5         ��Name$ CmbPort��5$      $��2��6   $��3��6/   �� 5%      -   5         ��Name$ TxtPort��5%      $��2��6   $y�Hidden=�6.   $��3��6B   �� 5&      -   5        $ CmbVelTrans��5&      $��3��6#   $��2��60   5'      �� 5(      -	   5         ��Name$ 
ImgElectro��5(   y   $
��Left=�6
   $��392,361,0,0q�6%   $	��Top=�6:   $#��images/Electrocardiograma.png��6R   $
��Fill=�6�   $��83��6�   $��82��6�   �� 5)      $
��Left=�6
   $��83��6%   $��547,361,0,0q�61   $	��Top=�6F   $��82��6^   $��images/glucometro.png��6i   $
��Fill=�6�   �� 5*      $
��Left=�6
   $��83��6%   $��700,361,0,0q�61   $	��Top=�6F   $��82��6^   $��images/pulsiometro.png��6i   $
��Fill=�6�   �� 5+      -
   5        $ ImgElectro_Copy��5+   n   $
��Left=�6
   $��232,361,0,0q�6%   $	��Top=�6:   $��images/medidas.png��6R   $
��Fill=�6�   $��83��6�   $��82��6�   �� 5,       �� Orientation$ 
Horizontal=�6   $
��Left=�6(   $��Bottom=�6C   .+�5-      �� -   5        $	 
TxtMensaje��5-      $�� ��6   $��14(�6+   $
��Bold"�69   ���	#FFF709096K   5.      �� 5/      -   5         ��Name$ 
BtnElectro��5/      $��Electrocardiograma��6   $
��Left=�6<   $��377,466,0,0q�6W   $	��Top=�6l   $	��114��6�   �� 50      -   5        $ BtnPulsiometro��50      $��Pulsiometro��6#   $
��Left=�69   $��708,466,0,0q�6T   $	��Top=�6i   $��74��6�   �� 51      -   5        $ BtnGlucometro��51      $��
Glucometro��6"   $
��Left=�67   $��551,466,0,0q�6R   $	��Top=�6g   $��74��6   �� 52      -   5        $ BtnNextMedida��52      $��=>��6"   $
��Left=�62   $��177,403,0,0q�6M   $	��Top=�6b   $��37��6z   �� 53      -   5        $ BtnNextElectro��53      $��=>��6#   $
��Left=�63   $��343,403,0,0q�6N   $	��Top=�6c   $��37��6{   �� 54      -   5        $ BtnNextGlucometro��54      $��=>��6&   $
��Left=�66   $��500,403,0,0q�6Q   $	��Top=�6f   $��37��6~   �� 55      -   5        $ BtnNextPulsiometro��55      $��=>��6'   $
��Left=�67   $��649,403,0,0q�6R   $	��Top=�6g   $��37��6   �� 56      -   5        $ 
BtnMedidas��56      $��Medidas��6   $
��Left=�61   $��237,466,0,0q�6L   $	��Top=�6a   $��74��6y   �� 57      -   5        $ 
BtnGuardar��57      $��Guardar��6   $
��Left=�61   $��560,140,0,0q�6L   $	��Top=�6a   $��74��6y   �� 58      -   5        $ 
BtnDetener��58      $��Detener��6   $
��Left=�61   $��212,139,0,0q�6L   $	��Top=�6a   $��74��6y   �� 59      -   5        $ BtnSalir��59      $��Salir��6   $
��Left=�6-   $��383,140,0,0q�6H   $	��Top=�6]   $��74��6u   �� 5:      -   5        $ BtnPeso��5:      $
��Peso��6   $
��Left=�6+   $��
83,466,0,0q�6F   $	��Top=�6Z   $��74��6r   �� 5;      -   5        $ BtnNextPeso��5;      $��=>��6    $
��Left=�60   $��
27,403,0,0q�6K   $	��Top=�6_   $��37��6w   �� 5<      -   5        $ ImgPeso��5<   j   $
��Left=�6
   $��
78,361,0,0q�6%   $	��Top=�69   $��images/peso.png��6Q   $
��Fill=�6{   $��83��6�   $��82��6�   �� 5=      -   5        $ Txtlog��5=      $�� ��6   $	��450��6$   $	��110��60    �� AcceptsReturn .  �� TextWrapping$
 Wrap=�6R   " �� VerticalScrollBarVisibility$
 Auto=�6f   $��196,190,189,258q�6�   �� 5>      -   5         ��Name$ LblNextRotation��5>   
   $��Proxima Rotación en: ��6#   $
��Left=�6C   $��32��6^   $��0,2,0,0q�6j   $	��Top=�6{   $	��380��6�   $��17.333(�6�   $
��Bold"�6�   ���	#FFF1F1276�   5?      5@      