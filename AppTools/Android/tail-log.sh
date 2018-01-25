#!bin/sh

P=$(cd `dirname $0`; pwd)
source $P/config.cfg

FILE=$1

num=1
sleep_time=0.5
# 2 hours
timeout=$[3600*2*2]

while [ $num -le $timeout ]
do
    if [ ! -f $FILE ]; then
        sleep $sleep_time
        continue
    fi
    tail $FILE

    #echo $num
    CHECK_RESULT=`cat $BUILD_PATH/build-bundles.log | grep 'Exiting batchmode successfully now' | wc -l`
    #echo "The check results: "$CHECK_RESULT
    if [ $CHECK_RESULT -eq 1 ]; then
        echo "BUILD SUCCESSFUL"
        exit 0
    else
        sleep $sleep_time
    fi
    let num++ 
done

echo "BUILD FAILTURE"
exit 1
