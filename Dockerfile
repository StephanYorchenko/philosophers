FROM ubuntu:latest

WORKDIR /home/project

RUN  git clone https://github.com/StephanYorchenko/philosophers -b master
