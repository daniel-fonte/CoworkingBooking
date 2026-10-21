#!/bin/bash

awslocal sqs create-queue --queue-name workspace-availability
awslocal sqs create-queue --queue-name refresh-cache