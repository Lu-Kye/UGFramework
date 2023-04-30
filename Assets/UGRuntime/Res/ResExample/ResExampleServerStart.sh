#!/bin/sh
path=$(cd `dirname $0`; pwd)
assets_path=$path/../../../StreamingAssets

cd $assets_path
pid=`ps aux | grep 8080 | grep -v grep | awk '{print $2}'`
if [ ! -z $pid ]; then
    kill -9 $pid
fi
python3 -m http.server 8080 &> /dev/null &
