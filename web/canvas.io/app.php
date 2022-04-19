<?php
$x=$_GET['x'];
$y=$_GET['y'];
$r=(int)$_GET['r'];
$g=(int)$_GET['g'];
$b=(int)$_GET['b'];
$img=imagecreatefrompng('today.png');
$color=imagecolorallocate($img, $r, $g, $b);
if(!imagesetpixel($img,$x,$y,$color)){
    die("fail");
}
$save = "today.png";
chmod($save,0755);
imagepng($img, $save, 0, NULL);
imagedestroy($img);
echo("ok");

?>