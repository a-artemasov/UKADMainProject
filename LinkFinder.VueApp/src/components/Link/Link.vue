<template>
  <div class="row border-bottom py-3">
    <div class="col-10 px-5">{{url}}</div>
    <div v-if="enableTime" class="col-2  text-center border-left">{{timeResponse}} ms</div>
  </div>
</template>

<script>
  export default {
    name: 'crawled-link',
    props: {
      url: {
        type: String,
        default: ""
      },
      timeResponse: {
        type: Number,
        default: 0,
      },
      enableTime: {
        type: Boolean,
        default: false,
      }, 
    },
  }
</script>
