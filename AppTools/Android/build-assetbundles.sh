#!/bin/sh

P=$(cd `dirname $0`; pwd)
source $P/config.cfg

# Kill unity progress
ps -ef | grep Unity | grep -v grep | awk '{print "kill -9 " $2}' | sh

LOG_FILE=$BUILD_PATH/build-bundles.log
rm -rf $LOG_FILE

Unity -projectPath $PROJECT_PATH -executeMethod UGFramework.Res.ResMenu.BuildAndroid -batchmode -quit -logFile $LOG_FILE \
    | sh tail-log.sh $LOG_FILE

