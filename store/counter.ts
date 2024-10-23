import {defineStore } from 'pinia'

export const useCounterStore = defineStore('counter',()=>{
  const count = ref(1);
  function increment(){
    count.value++;
  }
  return {count,increment}
})