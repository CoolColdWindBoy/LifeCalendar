<?php

//requst url: shitao.tech?method=
//method: login, save, load, 
$servername="localhost";
$username="lifecalendar";
$password="CZW8O3czw8o3";
$dbname = "lifecalendar";
$conn=new mysqli($servername,$username,$password,$dbname);
if($conn->connect_error){
    die("Fail");
}
$method=$_GET['method'];

if($method=="login"){
  $mail=$_GET['mail'];
  $pass=$_GET['pass'];
  $sql="SELECT * FROM user WHERE mail = '".$mail."'";
  $result=$conn->query($sql);
  if (mysqli_num_rows($result) > 0) {
    $row = $result->fetch_assoc();
      if($row['password']==$pass){
        echo "[True]&id=".$row['id'].
        "&birthYear=".$row['birthYear'].
        "&birthMonth=".$row['birthMonth'].
        "&birthDay=".$row['birthDay'].
        "&lifeExpectancy=".$row['lifeExpectancy'];
      }else{
        echo "Error: Password Wrong";
      }
  } else {
    echo "Error: User No Entry";
  }
}
else if($method=="load"){
  $mail=$_GET['mail'];
  $pass=$_GET['pass'];
  $sql="SELECT * FROM user WHERE mail = '".$mail."'";
  $result=$conn->query($sql);
  if (mysqli_num_rows($result) > 0) {
    $row = $result->fetch_assoc();
      if($row['password']==$pass){
        $jsonName=$row['jsonName'];
        $fileContent = file_get_contents(__DIR__."/json/".$jsonName.".json");
        if($fileContent==true){
          echo "!".$fileContent;
        }else{
          echo "Fail";
        }
      }else{
        echo "Error: Password Wrong";
      }
  } else {
    echo "Error: User No Entry";
  }
}
else if($method=="save"){
  $mail=$_GET['mail'];
  $pass=$_GET['pass'];
  $sql="SELECT * FROM user WHERE mail = '".$mail."'";
  $result=$conn->query($sql);
  if (mysqli_num_rows($result) > 0) {
    $row = $result->fetch_assoc();
      if($row['password']==$pass){
        $jsonName=$row['jsonName'];
        $json=$_GET['json'];
        $res = file_put_contents(__DIR__."/json/".$jsonName.".json", $json);
        if($res==true){
          echo "OK";
        }else{
          echo "Fail";
        }
      }else{
        echo "Error: Password Wrong";
      }
  } else {
    echo "Error: User No Entry";
  }
}




$conn->close();
?>