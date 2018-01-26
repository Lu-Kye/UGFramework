#!bin/sh

P=$(cd `dirname $0`; pwd)
FILE=$1
SUCCESS_FLAG=$2
FAILURE_FLAG=$3

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

    CHECK_RESULT=`cat $FILE | grep "$SUCCESS_FLAG" | wc -l`
    CHECK_FAILURE=`cat $FILE | grep "$FAILURE_FLAG" | wc -l`

    if [ $CHECK_RESULT -eq 1 ]; then
        echo "BUILD SUCCESSFUL"
        exit 0
    elif [ $CHECK_FAILURE -eq 1 ]; then
        echo "BUILD FAILTURE"
        exit 1
    else
        sleep $sleep_time
    fi
    let num++ 
done

echo "BUILD FAILTURE"
exit 1
