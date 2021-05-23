# wait for the database startup and initialization
sleep 25s

# start the process engine
# index.js --extensions-dir=/extensions --seed-dir=/processes

index.js $@